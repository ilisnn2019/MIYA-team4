using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PimDeWitte.UnityMainThreadDispatcher;
using Samples.Whisper;
using UnityEngine;

public class VoiceRecorder
{
    private const int MAX_RECORD_TIME = 60;
    private const float SILENCE_THRESHOLD = 0.01f;
    private const float SILENCE_DURATION = 2.5f;

    private AudioClip recordedClip;
    private string filePath;
    private bool isRecording = false;
    private float silenceTimer = 0f;
    private float maxTimer = 0f;
    private int sampleSize = 256;

    // 외부에서 접근 가능한 현재 볼륨 (0~1 범위)
    public static float CurrentVolume { get; private set; } = 0f;

    public event Action OnRecordingStopped;

    public VoiceRecorder()
    {
        filePath = Path.Combine(Application.dataPath, "Personals/audio.wav");
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("<color=red>[VoiceRecord]</color> No microphone found!");
            return;
        }

        Debug.Log("<color=cyan>[VoiceRecord]</color> Recording started!");
        isRecording = true;
        silenceTimer = 0f;
        maxTimer = 0f;
        recordedClip = Microphone.Start(null, false, MAX_RECORD_TIME, 16000);
        UnityMainThreadDispatcher.Instance().Enqueue(CheckSilenceAndStop());
    }

    private IEnumerator CheckSilenceAndStop()
    {
        float[] samples = new float[sampleSize];

        while (isRecording)
        {
            if (recordedClip == null || !Microphone.IsRecording(null))
                yield break;

            int position = Microphone.GetPosition(null);
            if (position < sampleSize)
                continue;

            recordedClip.GetData(samples, position - sampleSize);

            // 볼륨 계산
            float sum = 0f;
            for (int i = 0; i < samples.Length; i++)
                sum += samples[i] * samples[i];
            float rms = Mathf.Sqrt(sum / samples.Length);
            CurrentVolume = Mathf.Clamp01(rms * 10f); // 감도 보정 (임의 배수)

            bool hasSound = rms > SILENCE_THRESHOLD;

            if (hasSound)
                silenceTimer = 0f;
            else
                silenceTimer += 0.1f;

            maxTimer += 0.1f;

            GText.PrintState($"Record Voice, silence timer : {silenceTimer}, vol : {CurrentVolume:F2}");

            if (silenceTimer >= SILENCE_DURATION || maxTimer >= 15f)
            {
                StopRecording();
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StopRecording()
    {
        if (!Microphone.IsRecording(null))
        {
            Debug.LogWarning("<color=yellow>[VoiceRecord]</color> No recording in progress.");
            return;
        }

        isRecording = false;
        Microphone.End(null);
        Debug.Log("<color=cyan>[VoiceRecord]</color> Recording stopped. Saving...");

        // 볼륨 리셋
        CurrentVolume = 0f;

        // 무음 제거 후 WAV 저장
        var trimmed = SaveWav.TrimSilence(recordedClip, SILENCE_THRESHOLD);
        byte[] wavData = SaveWav.Save(Path.GetFileName(filePath), trimmed);

        File.WriteAllBytes(filePath, wavData);
        Debug.Log("<color=cyan>[VoiceRecord]</color> Saved to: " + filePath);

        OnRecordingStopped?.Invoke();
    }
}

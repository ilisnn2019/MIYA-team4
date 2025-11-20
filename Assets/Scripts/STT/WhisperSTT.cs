using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenAI;
using OVR.OpenVR;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.Networking;

public class WhisperSTT : ISTT
{
    private string filePath;

    private const string HEAD = "STT";

    private readonly OpenAIApi openai;

    public WhisperSTT(string key)
    {
        openai = new OpenAIApi(apiKey:key);
    }

    [ContextMenu("Call STT")]
    public void SpeechtoText(Action<string> callback)
    {
        filePath = Path.Combine(Application.dataPath, "Personals/audio.wav");
        UnityMainThreadDispatcher.Instance().Enqueue(UploadAudioAndGetText(callback));
    }

    IEnumerator UploadAudioAndGetText(Action<string> callback)
    {
        if (!File.Exists(filePath))
        {
            LLog.Log(LogType.Error, HEAD, $"File not found: {filePath}");
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        LLog.Log(LogType.Log, HEAD, "Audio file size: " + audioData.Length);

        // Whisper 요청 생성
        var request = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData
            {
                Data = audioData,
                Name = "audio.wav"
            },
            Model = "whisper-1",
            Language = "ko",          // 한국어로 지정
            Temperature = 0f          // 덧붙임 억제
        };

        // 비동기 호출 래핑
        var task = openai.CreateAudioTranscription(request);

        // 기다림
        while (!task.IsCompleted) yield return null;

        if (task.IsFaulted)
        {
            LLog.Log(LogType.Error, HEAD, "Transcription failed: " + task.Exception);
            callback?.Invoke(null);
        }
        else
        {
            var response = task.Result;
            LLog.Log(LogType.Log, HEAD, "Whisper response: " + response.Text);
            callback?.Invoke(response.Text);
        }
    }
}

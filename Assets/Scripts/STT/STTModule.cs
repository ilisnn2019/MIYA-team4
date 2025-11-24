using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using WakeWordModule;

public enum STTType
{
    Clova,
    Whisper
}

public class STTModule : MonoBehaviour
{
    [SerializeField] AccessKeySO accessKeySO;
    VoiceRecorder _recorder;
    private ISTT stt;
    [SerializeField] private STTType _sttType; // Inspector 표시
    public STTType stt_type
    {
        get => _sttType;
        set
        {
            if (_sttType != value)
            {
                _sttType = value;
                InitSTT();
            }
        }
    }

    [Tooltip("Unity Event that delivers the string value returned after STT.")]
    public UnityEvent<string> _onSTTResponseCallback;
    public UnityEvent _onSTTResponseErrorHandler;
    [TextArea(3, 10)]
    [SerializeField] private string _lastResponse = "";

    private void Start()
    {
        _recorder = new VoiceRecorder();
        InitSTT();

        _recorder.OnRecordingStopped += () =>
        {
#if UNITY_EDITOR
            TimerLogger.allowSave = true;
            GText.PrintState("STT Processing ... ");
            TimerLogger.StartTimer();
#endif
            stt.SpeechtoText(OnSTTResponseReceived);
        };
    }

    private void InitSTT()
    {
        if (_sttType == STTType.Clova)
        {
            stt = new ClovaSTT();
        }
        else
        {
            stt = new WhisperSTT(accessKeySO.accessKey);
        }

#if UNITY_EDITOR
        Debug.Log($"[STTModule] STT 모델이 변경되었습니다: {_sttType}");
#endif
    }

    public void StartRecording() => _recorder.StartRecording();

    public void ForceApplySTT() => InitSTT(); // 버튼에서 호출할 메서드

    private void OnSTTResponseReceived(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("Bad STT");
            _lastResponse = "<Bad STT Response>";
            _onSTTResponseErrorHandler?.Invoke();
            return;
        }

        _lastResponse = response;  // 받은 텍스트 저장

#if UNITY_EDITOR
        TimerLogger.EndTimer("STT");
        GText.SaveLine("STT", "STT 결과(전체 텍스트) : " + response);
        GText.PrintState("Sending to GPT and Wait Reponse ... ");
#endif

        _onSTTResponseCallback?.Invoke(response);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            InitSTT(); // Inspector에서 변경 시 즉시 반영
        }
    }
}

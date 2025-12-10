using System;
using System.Collections;
using System.Reflection;
using Meta.WitAi;
using Meta.WitAi.CallbackHandlers;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Json;
using Meta.WitAi.Lib;
using Meta.WitAi.Requests;
using Oculus.Voice;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class VoiceDetector : MonoBehaviour
{
    [Header("Wit.ai Components")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private WitResponseMatcher witResponseMatcher;

    [Header("Mic Control")]
    private Mic mic;
    private MethodInfo startMicMethod;

    [Header("Voice Events")]
    [SerializeField] private UnityEvent onWordDetected;
    [SerializeField] private UnityEvent onStartListening;

    [SerializeField] private bool voiceCommandReady = false;

    private void Awake()
    {
        SetupMicReflection();
        SubscribeVoiceEvents();
        appVoiceExperience.Activate();
    }

    private void Start()
    {
        mic = (Mic)AudioBuffer.Instance.MicInput;
    }

    private void OnDestroy()
    {
        UnsubscribeVoiceEvents();
    }

    #region Setup

    private void SetupMicReflection()
    {
        // Mic 클래스의 private StartMicrophone 메서드를 찾음
        startMicMethod = typeof(Mic).GetMethod("StartMicrophone", BindingFlags.Instance | BindingFlags.NonPublic);
        if (startMicMethod == null)
        {
            Debug.LogError("StartMicrophone method not found in Mic class!");
        }
    }

    private void SubscribeVoiceEvents()
    {
        if (appVoiceExperience == null) return;

        appVoiceExperience.VoiceEvents.OnMicStartedListening.AddListener(OnMicStarted);
        appVoiceExperience.VoiceEvents.OnComplete.AddListener(OnRequestComplete);
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(OnRequestCompleted);
        appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnFullTranscription);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveAllListeners();

        appVoiceExperience.VoiceEvents.OnCanceled.RemoveAllListeners();
        appVoiceExperience.VoiceEvents.OnCanceled.AddListener(Oncanceled);
        appVoiceExperience.VoiceEvents.OnError.AddListener(OnError);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField?.GetValue(witResponseMatcher) is MultiValueEvent multiValueEvent)
        {
            multiValueEvent.AddListener(WakeWordDetected);
        }
    }
    private void UnsubscribeVoiceEvents()
    {
        if (appVoiceExperience == null) return;

        appVoiceExperience.VoiceEvents.OnMicStartedListening.RemoveListener(OnMicStarted);
        appVoiceExperience.VoiceEvents.OnComplete.RemoveListener(OnRequestComplete);
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(OnRequestCompleted);
        appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnFullTranscription);
        //appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);

        appVoiceExperience.VoiceEvents.OnCanceled.RemoveListener(Oncanceled);
        appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnError);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField?.GetValue(witResponseMatcher) is MultiValueEvent multiValueEvent)
        {
            multiValueEvent.RemoveListener(WakeWordDetected);
        }
    }

    #endregion

    #region Voice Event Handlers

    private void Oncanceled(string arg0)
    {
        Debug.LogWarning("voice detector catch canceld");
        appVoiceExperience.DeactivateAndAbortRequest();
        appVoiceExperience.Activate();
    }

    private void OnError(string error, string message)
    {
        Debug.LogError($"[VoiceDetector] Error: {error}");
        if (!voiceCommandReady)
        {
            StartCoroutine(RestartWitRoutine());
        }
    }

    private void OnMicStarted()
    {
        GText.PrintState("Try to Catch Wake Word ... ");
        Debug.Log("[VoiceDetector] Mic ready to listen.");
    }

    private void OnRequestComplete(VoiceServiceRequest request)
    {
        // 이미 웨이크워드를 찾았으면 재시작 안 함
        if (voiceCommandReady) return;

        Debug.Log("[VoiceDetector] Session ended. Restarting shortly...");

        // 즉시 재시작하지 말고, 코루틴으로 딜레이를 줌
        StartCoroutine(RestartWitRoutine());
    }

    private IEnumerator RestartWitRoutine()
    {
        // 0.5초 대기: 이 시간이 있어야 유니티가 멈추지 않음
        yield return new WaitForSeconds(0.1f);

        if (!voiceCommandReady)
        {
            // 리플렉션 없이 정석대로 실행
            appVoiceExperience.Activate();
        }
    }

    private void OnRequestCompleted()
    {
        Debug.Log("[VoiceDetector] Request completed.");
    }

    private void OnPartialTranscription(string partialText)
    {
        //if (statusText != null)
        //    statusText.text = $"…{partialText}";
    }

    private void OnFullTranscription(WitResponseNode response)
    {
        if (voiceCommandReady) return;
        if (response.GetIntents().Length <= 0) return;
        WitIntentData responseData = response.GetIntents()[0];
        if (responseData.name.Equals("not_wake_word")) return;
        float confidence = responseData.confidence;
        if (confidence < 0.8f) return;
        Debug.Log($"text : {response["text"]}, response name : {responseData.name}, confidence : {responseData.confidence}");
        GText.PrintState("Catch the word!");
        GText.SaveLine("WakeWord", "Wake Word 감지 결과 : " + response["text"] + "\n Confidence : " + confidence);
    }

    public void WakeWordDetected(string[] detectedWords)
    {
        if (voiceCommandReady) return;

        voiceCommandReady = true;
        Debug.Log($"[VoiceDetector] Wake word detected: {detectedWords?.Length} words");
        onWordDetected?.Invoke();
    }

    #endregion

    #region Public API

    /// <summary>
    /// 콜백 이후 다시 웨이크워드 탐지 상태로 초기화
    /// </summary>
    [ContextMenu("start detecting")]
    public void OnStartDetecting()
    {
        voiceCommandReady = false;
        onStartListening?.Invoke();
        ActivateMic();
    }

    #endregion

    #region Private Helpers

    private void ActivateMic()
    {
        appVoiceExperience.DeactivateAndAbortRequest();
        if (voiceCommandReady) return;
        if (mic != null && startMicMethod != null)
        {
            startMicMethod.Invoke(mic, null);
        }
        else
        {
            Debug.LogError("[VoiceDetector] Cannot activate mic.");
        }
        appVoiceExperience.Activate();
    }

    #endregion

}

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class ChatManager : MonoBehaviour
{
    private IOpenAI openAI;

    [SerializeField] AccessKeySO accessKeySO;

    [SerializeField]
    public UnityEvent<string> _OnResponseReceived;

    public string prompt_name;

    private void Start()
    {
        openAI = new GPT(accessKeySO, prompt_name);
    }

    /// <summary>
    /// calling after stt processing.
    /// </summary>
    /// <param name="input">stt result</param>
    public void OnSendMessage(string input)
    {

        string userMessage = input;
        if (!string.IsNullOrEmpty(userMessage))
        {
            StartCoroutine(openAI.SendMessage(userMessage, OnResponseReceived));
        }
    }

    private void OnResponseReceived(List<FunctionCall> response)
    {
        string json = JsonConvert.SerializeObject(response, Formatting.Indented); // Newtonsoft.Json 기준

#if UNITY_EDITOR
        GText.SaveLine("Json", "분석된 Command JSON : \n" + json);
        GText.PrintState("Recieve Response, Send to Command Dispatcher");
        TimerLogger.StartTimer();
#endif
        Debug.Log(json);

        _OnResponseReceived?.Invoke(json);
    }

#if UNITY_EDITOR
    [Header("Editor Input for test")]
    public string editorInput;
    [ContextMenu("Send Editor Input")]
    public void SendUserInput()
    {
        OnSendMessage(editorInput);
    }

    [ContextMenu("get prev cmd")]
    public void GetPrevCMD()
    {
        string prev = InterfaceContainer.Resolve<IStorage>("storage").GetChanged();
        LLog.Log(LogType.Log, "STG", prev);
    }
#endif

}

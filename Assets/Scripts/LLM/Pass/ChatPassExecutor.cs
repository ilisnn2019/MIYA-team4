using System;
using System.Collections;
using UnityEngine.Networking;

public abstract class ChatPassExecutor<T>
{
    protected readonly string apiKey;
    protected readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

    protected ChatPassExecutor(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public abstract IEnumerator Execute(string userInput, Action<T> onComplete);

    protected UnityWebRequest CreateRequest(string payload)
    {
        var request = new UnityWebRequest(apiUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(payload));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        return request;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System;
using PimDeWitte.UnityMainThreadDispatcher;
using OpenAI;

public interface ISTT
{
    public void SpeechtoText(Action<string> callback);
}


public class ClovaSTT : ISTT
{
    private string clientId = "gwnzep9q1f";  // 네이버 클라우드 플랫폼에서 발급받은 Client ID
    private string clientSecret = "RAUlNHpsCUnEZeQO1DLiaC9c6IqLEaWDO8yQZLDo";  // Client Secret

    private string apiURL = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor";

    private string filePath;

    private const string HEAD = "STT";

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

        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
        {
            LLog.Log(LogType.Error, HEAD, $"Error: The file is empty!");
            yield break;
        }

        LLog.Log(LogType.Log, HEAD, "File size: " + fileInfo.Length + " bytes");

        byte[] audioData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddField("language", "ko-KR");
        form.AddField("completion", "sync");
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");

        UnityWebRequest request = UnityWebRequest.Post(apiURL, form);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", clientId);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", clientSecret);
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        request.uploadHandler = new UploadHandlerRaw(audioData);

        yield return request.SendWebRequest();

        LLog.Log(LogType.Log, HEAD, "Response Code: " + request.responseCode);
        LLog.Log(LogType.Log, HEAD, "Response Body: " + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
        {
            LLog.Log(LogType.Error, HEAD, "Error:" + request.error);
            callback?.Invoke(null);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            LLog.Log(LogType.Log, "STT", "Response: " + jsonResponse);
            callback?.Invoke(jsonResponse);     
        }
    }
}

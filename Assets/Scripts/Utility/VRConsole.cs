using UnityEngine;
using System;
using System.IO;

public class LogSaver : MonoBehaviour
{
    private string logFilePath;
    private StreamWriter writer;

    void Awake()
    {
        // 저장 위치: persistentDataPath 아래
        string folderPath = Path.Combine(Application.persistentDataPath, "Logs");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(folderPath, $"log_{timestamp}.txt");

        writer = new StreamWriter(logFilePath, true);
        writer.AutoFlush = true;

        Application.logMessageReceived += OnLogMessage;
    }

    private void OnLogMessage(string condition, string stackTrace, UnityEngine.LogType type)
    {
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string log = $"[{time}] [{type}] {condition}";

        writer.WriteLine(log);

        // 에러일 경우 스택트레이스도 기록
        if (type.Equals(LogType.Error) || type.Equals(LogType.Warning))
            writer.WriteLine(stackTrace);
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessage;

        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }
}

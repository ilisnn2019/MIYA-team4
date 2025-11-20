using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public enum LogType
{
    Log,
    Warning,
    Error
}

public static class LLog
{
    private static Dictionary<LogType, string> typeColors = new Dictionary<LogType, string>
    {
        { LogType.Log, "cyan" },
        { LogType.Warning, "yellow" },
        { LogType.Error, "red" }
    };

    public static bool SaveToFile = false;
    private static readonly string logFilePath = Path.Combine(Application.persistentDataPath, "LLog.txt");

    // 메인 함수
    public static void Log(
        LogType type,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string fileName = Path.GetFileName(sourceFilePath);
        string header = $"{fileName}:{memberName}({sourceLineNumber})";

        string color = typeColors.ContainsKey(type) ? typeColors[type] : "white";
        string formattedMessage = $"<color={color}>[{header}]</color> {message}";

        switch (type)
        {
            case LogType.Log:
                Debug.Log(formattedMessage);
                break;
            case LogType.Warning:
                Debug.LogWarning(formattedMessage);
                break;
            case LogType.Error:
                Debug.LogError(formattedMessage);
                break;
        }

        if (SaveToFile)
        {
            SaveLogToFile(type, header, message);
        }
    }

    // 헤더 따로 넘기는 간단한 오버로드
    public static void Log(LogType type, string head, string message)
    {
        string color = typeColors.ContainsKey(type) ? typeColors[type] : "white";
        string formattedMessage = $"<color={color}>[{head}]</color> {message}";

        switch (type)
        {
            case LogType.Log:
                Debug.Log(formattedMessage);
                break;
            case LogType.Warning:
                Debug.LogWarning(formattedMessage);
                break;
            case LogType.Error:
                Debug.LogError(formattedMessage);
                break;
        }

        if (SaveToFile)
        {
            SaveLogToFile(type, head, message);
        }
    }

    // 파일 저장
    private static void SaveLogToFile(LogType type, string head, string message)
    {
        try
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timeStamp}] [{type}] [{head}] {message}\n";
            File.AppendAllText(logFilePath, logEntry);
        }
        catch (Exception ex)
        {
            Debug.LogError($"<color=red>[LLog]</color> Failed to save log file: {ex.Message}");
        }
    }
}

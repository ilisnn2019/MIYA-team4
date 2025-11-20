using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class TimerLogger
{
    public static bool allowSave = true;
    private static Stopwatch stopwatch = new Stopwatch();
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "TimerLog.txt"); // AppData/LocalLow/DefaultCompany/VR

    /// <summary>
    /// 타이머를 시작합니다.
    /// </summary>
    public static void StartTimer()
    {
        if (!allowSave) return;
        stopwatch.Reset();
        stopwatch.Start();
    }

    /// <summary>
    /// 타이머를 종료하고, 경과 시간을 파일에 기록합니다.
    /// </summary>
    /// <param name="label">기록할 이름</param>
    public static void EndTimer(string label)
    {
        if (!allowSave) return;
        stopwatch.Stop();
        string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {label} | {stopwatch.ElapsedMilliseconds} ms";

        //Debug.Log(log);
        SaveLog(log);
    }

    private static void SaveLog(string log)
    {
        try
        {
            File.AppendAllText(logFilePath, log + Environment.NewLine);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to save log: {e.Message}");
        }
    }
}

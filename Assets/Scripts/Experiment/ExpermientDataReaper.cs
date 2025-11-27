using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ExpermientDataReaper : MonoBehaviour
{
    enum DATA_LABEL
    {
        STT, TASK, ENTIRE_TASK
    }

    Dictionary<DATA_LABEL, Stopwatch> timers = new();

    private string logFilePath;
    private string content = "";
    private string date = "";
    private void Start()
    {
        date = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        logFilePath = Path.Combine(Application.persistentDataPath, $"Experiment-{date}.txt");

        timers = new() { { DATA_LABEL.STT, new Stopwatch() }, { DATA_LABEL.TASK, new Stopwatch() }, { DATA_LABEL.ENTIRE_TASK, new Stopwatch() } };
    }

    private bool isSTT = false;

    public void StartSTT()
    {
        if (!isSTT) { content += "("; isSTT = true; }
        timers[DATA_LABEL.STT].Reset();
        timers[DATA_LABEL.STT].Start();
    }

    public void EndSTT(bool isEnd = false)
    {
        var sw = timers[DATA_LABEL.STT];
        if (sw.IsRunning)
        {
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            content += ms;
        }
        if (!isEnd) content += ",";
        else { content += "),"; isSTT = false;  }
    }

    public void StartTask()
    {
        content += "(\n";
        timers[DATA_LABEL.TASK].Reset();
        timers[DATA_LABEL.TASK].Start();
    }

    public void EndTask()
    {
        var sw = timers[DATA_LABEL.TASK];
        if (sw.IsRunning)
        {
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            content += ms;
        }
        content += "),";
    }

    public void StartEntireTask()
    {
        content += "(\n";
        timers[DATA_LABEL.ENTIRE_TASK].Reset();
        timers[DATA_LABEL.ENTIRE_TASK].Start();
    }
    public void EndEntireTask()
    {
        var sw = timers[DATA_LABEL.ENTIRE_TASK];
        if (sw.IsRunning)
        {
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            content += ms;
        }
        content += ")";

        try
        {
            File.WriteAllText(logFilePath, content);
            UnityEngine.Debug.Log($"Experiment data saved to: {logFilePath}");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to save experiment data: {e.Message}");
        }
    }

}

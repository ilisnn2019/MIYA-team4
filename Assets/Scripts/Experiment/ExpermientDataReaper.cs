using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

[Serializable]
public class STTRecord
{
    public long time;
    public string result;
}

[Serializable]
public class TaskRecord
{
    public long taskTime;
    public List<STTRecord> sttRecords = new List<STTRecord>();
}

[Serializable]
public class ExperimentData
{
    public long entireTaskTime;
    public List<TaskRecord> tasks = new List<TaskRecord>();
}

public class ExpermientDataReaper : MonoBehaviour
{
    enum DATA_LABEL { STT, TASK, ENTIRE_TASK }

    Dictionary<DATA_LABEL, Stopwatch> timers = new();

    private string logFilePath;
    private ExperimentData experimentData;

    private TaskRecord currentTask;
    private STTRecord currentSTT;

    private void Start()
    {
        string date = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        logFilePath = Path.Combine(Application.persistentDataPath, $"Experiment-{date}.json");

        timers = new()
        {
            { DATA_LABEL.STT, new Stopwatch() },
            { DATA_LABEL.TASK, new Stopwatch() },
            { DATA_LABEL.ENTIRE_TASK, new Stopwatch() }
        };

        experimentData = new ExperimentData();
    }

    // =============================
    //      STT
    // =============================

    public void StartSTT(string sttResult)
    {
        currentSTT = new STTRecord();
        currentSTT.result = sttResult;

        timers[DATA_LABEL.STT].Reset();
        timers[DATA_LABEL.STT].Start();
    }

    public void EndSTT()
    {
        var sw = timers[DATA_LABEL.STT];
        if (sw.IsRunning)
        {
            sw.Stop();
            currentSTT.time = sw.ElapsedMilliseconds;
        }

        currentTask.sttRecords.Add(currentSTT);
        currentSTT = null;
    }

    // =============================
    //      Task
    // =============================

    public void StartTask()
    {
        currentTask = new TaskRecord();

        timers[DATA_LABEL.TASK].Reset();
        timers[DATA_LABEL.TASK].Start();
    }

    public void EndTask()
    {
        var sw = timers[DATA_LABEL.TASK];
        if (sw.IsRunning)
        {
            sw.Stop();
            currentTask.taskTime = sw.ElapsedMilliseconds;
        }

        experimentData.tasks.Add(currentTask);
        currentTask = null;
    }

    // =============================
    //      Entire Task
    // =============================

    public void StartEntireTask()
    {
        timers[DATA_LABEL.ENTIRE_TASK].Reset();
        timers[DATA_LABEL.ENTIRE_TASK].Start();
    }

    public void EndEntireTask()
    {
        var sw = timers[DATA_LABEL.ENTIRE_TASK];
        if (sw.IsRunning)
        {
            sw.Stop();
            experimentData.entireTaskTime = sw.ElapsedMilliseconds;
        }

        SaveData();
    }

    // =============================
    //      SAVE
    // =============================

    private void SaveData()
    {
        try
        {
            string json = JsonUtility.ToJson(experimentData, true);
            File.WriteAllText(logFilePath, json);
            UnityEngine.Debug.Log($"Experiment data saved to: {logFilePath}");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to save experiment data: {e.Message}");
        }
    }
}

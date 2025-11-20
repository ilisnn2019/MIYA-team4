using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GText : MonoBehaviour
{
    private static TextMeshProUGUI state_text;
    private static TextMeshProUGUI text;
    private static Dictionary<string, string> lines;

    private void Awake()
    {
        var texts = GetComponentsInChildren<TextMeshProUGUI>();
        state_text = texts[0].GetComponent<TextMeshProUGUI>();
        text = texts[1].GetComponent<TextMeshProUGUI>();
        Initialize();
    }

    private static void Initialize()
    {
        lines = new Dictionary<string, string>
        {
            { "WakeWord", "" },
            { "STT", "" },
            { "Json", "" },
            { "Output", "" },
        };
        UpdateText("");
    }

    /// <summary>
    /// 특정 키에 값을 저장하거나 갱신합니다.
    /// </summary>
    public static void SaveLine(string key, string line)
    {
        if (lines == null) Initialize();

        lines[key] = line;
    }

    /// <summary>
    /// 저장된 모든 라인을 출력하고 초기화합니다.
    /// </summary>
    public static void PrintLines()
    {
        if (text == null || lines == null) return;
        Clear();
        var output = "";
        foreach (var kvp in lines)
        {
            output += $"[{kvp.Key}]\n {kvp.Value}\n";
        }

        UpdateText(output);
    }

    /// <summary>
    /// 현재 상태를 즉시 출력합니다.
    /// </summary>
    public static void PrintState(string state)
    {
        state_text.text =state;
    }

    /// <summary>
    /// 모든 텍스트를 초기화합니다.
    /// </summary>
    public static void Clear()
    {
        UpdateText("");
    }

    /// <summary>
    /// 특정 키를 가진 라인을 삭제합니다.
    /// </summary>
    public static void RemoveLine(string key)
    {
        if (lines == null) return;

        lines.Remove(key);
    }

    private static void UpdateText(string value)
    {
        if (text != null)
            text.text = value;
    }
}

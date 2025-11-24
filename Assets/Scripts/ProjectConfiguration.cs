using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ProjectConfiguration : MonoBehaviour
{
    public VoiceDetector voicedetector;
    public STTModule sttmodule;
    public ChatManager chatManager;
    public CommandExecutor executor;

    public TextAsset promptFile; // 프롬프트 텍스트 파일 (.txt)
    public EntityPack objectListFile; // 오브젝트 타입이 적힌 외부 파일 -> scriptableobject로 변경
    public TexturePack textureListFile; // 텍스쳐 리스트가 적힌 외부 파일  -> scriptableobject로 변경
    public string newPromptName;

    public void UpdatePromptFile()
    {
        string promptPath = AssetDatabase.GetAssetPath(promptFile);
        string promptText = File.ReadAllText(promptPath, Encoding.UTF8);

        string objectTypes = "(none)";
        if (objectListFile != null && objectListFile.entities != null && objectListFile.entities.Count > 0)
        {
            objectTypes = string.Join("\n", objectListFile.entities.Select(e => e.entityType));
        }

        string textures = "(none)";
        if (textureListFile != null && textureListFile.textureSpec != null && textureListFile.textureSpec.Count > 0)
        {
            textures = string.Join("\n", textureListFile.textureSpec.Select(e => e.identifier));
        }

        // 특수 표시를 찾아 교체
        promptText = promptText.Replace("/*object_types_here*/", "\n" + objectTypes);
        promptText = promptText.Replace("/*textures_here*/", "\n" + textures);

        // --- 새 파일 저장 경로 ---
        string resourcesPath = "Assets/Resources";
        if (!Directory.Exists(resourcesPath))
            Directory.CreateDirectory(resourcesPath);

        string newFileName = string.IsNullOrEmpty(newPromptName) ? "UpdatedPrompt" : newPromptName;
        string newFilePath = Path.Combine(resourcesPath, $"{newFileName}.txt");

        File.WriteAllText(newFilePath, promptText, Encoding.UTF8);
        AssetDatabase.Refresh();

        chatManager.prompt_name = newPromptName;

        Debug.Log($"Prompt file updated at: {promptPath}");
    }

    [SerializeField]
    private EntityInfoAgent[] entities;

    private void Start()
    {
        // registry initialize in Awake step.
        IRegistry registry = InterfaceContainer.Resolve<IRegistry>("registry");

        entities = GameObject.FindObjectsByType<EntityInfoAgent>(FindObjectsSortMode.None);

        foreach (var entity in entities)
        {
            entity.Initialize("");
        }

        executor.SetConfiguration(objectListFile, textureListFile);    
    }

}

[CustomEditor(typeof(ProjectConfiguration))]
public class ProjectConfigurationEditor : Editor
{
    SerializedProperty voicedetector;
    SerializedProperty sttmodule;
    SerializedProperty chatManager;
    SerializedProperty executor;

    SerializedProperty promptFile;
    SerializedProperty objectListFile;
    SerializedProperty textureListFile;
    SerializedProperty newPromptName;

    void OnEnable()
    {
        voicedetector = serializedObject.FindProperty("voicedetector");
        sttmodule = serializedObject.FindProperty("sttmodule");
        chatManager = serializedObject.FindProperty("chatManager");
        executor = serializedObject.FindProperty("executor");

        promptFile = serializedObject.FindProperty("promptFile");
        objectListFile = serializedObject.FindProperty("objectListFile");
        textureListFile = serializedObject.FindProperty("textureListFile");
        newPromptName = serializedObject.FindProperty("newPromptName");

    }
    public override void OnInspectorGUI()
    {
        // Header 스타일
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.normal.textColor = Color.white;
        serializedObject.Update();

        ProjectConfiguration config = (ProjectConfiguration)target;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Settings", headerStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(voicedetector);
        EditorGUILayout.PropertyField(sttmodule);
        EditorGUILayout.PropertyField(chatManager);
        EditorGUILayout.PropertyField(executor);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();

        DrawSeparator();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Prompt Settings", headerStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("“Reflect the object types and textures needed for the project in the prompt.\nThe prompt must be registered in ChatManager.”", MessageType.Info);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(promptFile);
        EditorGUILayout.PropertyField(objectListFile);
        EditorGUILayout.PropertyField(textureListFile);
        EditorGUILayout.PropertyField(newPromptName);
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Generate prompt file"))
        {
            config.UpdatePromptFile();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSeparator()
    {
        GUILayout.Space(5);
        GUIStyle lineStyle = new GUIStyle();
        lineStyle.normal.background = Texture2D.whiteTexture;
        lineStyle.fixedHeight = 1;
        lineStyle.margin = new RectOffset(0, 0, 4, 4);
        Color oldColor = GUI.color;
        GUI.color = Color.gray; // 선 색상
        GUILayout.Box(GUIContent.none, lineStyle);
        GUI.color = oldColor;
        GUILayout.Space(5);
    }
}
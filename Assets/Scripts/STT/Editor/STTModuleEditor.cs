// Assets/Editor/STTModuleEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(STTModule))]
public class STTModuleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        STTModule sttModule = (STTModule)target;

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Apply STT Model Now"))
            {
                sttModule.ForceApplySTT();
            }
        }
    }
}

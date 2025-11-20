using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CommandExecutor))]
public class CommandExecutorEditor : Editor
{
    const string ID = "TESTCUBE00";

    // 인스펙터에서 조절할 파라미터들
    private Vector3 moveTarget = Vector3.one;
    private GameObject moveObject; 
    private float rotateAngle = 30f;
    private float scaleFactor = 1.5f;
    private string colorHex = "#FF0000";
    private string textureName = "wood";
    private float weight = 3f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CommandExecutor cme = (CommandExecutor)target;

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Create Test Cube", EditorStyles.boldLabel);
            if (GUILayout.Button("create_object"))
            {
                cme.create_object("beach_ball", ID);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Move Settings", EditorStyles.boldLabel);
            moveTarget = EditorGUILayout.Vector3Field("Target Position", moveTarget);
            moveObject = EditorGUILayout.ObjectField(moveObject, typeof(GameObject), false) as GameObject;
            if (GUILayout.Button("move_object"))
            {
                if (moveObject != null) {
                    Vector3 pos = moveObject.transform.position;
                    cme.move_object(ID, pos.x,pos.y,pos.z); 
                }
                else cme.move_object(ID, moveTarget.x, moveTarget.y, moveTarget.z);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Rotate Settings", EditorStyles.boldLabel);
            rotateAngle = EditorGUILayout.FloatField("Angle", rotateAngle);
            if (GUILayout.Button("rotate_object"))
            {
                cme.rotate_object(ID, rotateAngle);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Scale Settings", EditorStyles.boldLabel);
            scaleFactor = EditorGUILayout.FloatField("Scale Factor", scaleFactor);
            if (GUILayout.Button("scale_object"))
            {
                cme.scale_object(ID, scaleFactor);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Appearance Settings", EditorStyles.boldLabel);
            colorHex = EditorGUILayout.TextField("Color (Hex)", colorHex);
            if (GUILayout.Button("set_color"))
            {
                cme.set_color(ID, colorHex);
            }

            textureName = EditorGUILayout.TextField("Texture Name", textureName);
            if (GUILayout.Button("set_texture"))
            {
                cme.set_texture(ID, textureName);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);
            weight = EditorGUILayout.FloatField("Weight", weight);
            if (GUILayout.Button("set_weight"))
            {
                cme.set_weight(ID, weight);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Remove Test Cube", EditorStyles.boldLabel);
            if (GUILayout.Button("remove_object"))
            {
                cme.remove_object(ID);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 활성화됩니다.", MessageType.Info);
        }
    }
}

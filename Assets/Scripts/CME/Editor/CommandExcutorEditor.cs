using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CommandExecutor))]
public class CommandExecutorEditor : Editor
{
    // 생성 타입 옵션
    private string[] objectTypes = { "sphere", "cube", "cylinder" };
    private int selectedObjectTypeIndex = 0;

    // 사용자 입력 ID
    private string targetID = "TESTCUBE00";

    // 파라미터들
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
            // ID 입력
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Target Object ID", EditorStyles.boldLabel);
            targetID = EditorGUILayout.TextField("ID", targetID);

            //----------------------------------------
            // 1. 객체 생성
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Create Object", EditorStyles.boldLabel);

            selectedObjectTypeIndex = EditorGUILayout.Popup("Object Type", selectedObjectTypeIndex, objectTypes);

            if (GUILayout.Button("create_object"))
            {
                string typeToCreate = objectTypes[selectedObjectTypeIndex];
                cme.create_object(typeToCreate, targetID);
            }

            //----------------------------------------
            // 2. 이동
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Move Settings", EditorStyles.boldLabel);
            moveTarget = EditorGUILayout.Vector3Field("Target Position", moveTarget);
            moveObject = EditorGUILayout.ObjectField("Use Object Position", moveObject, typeof(GameObject), false) as GameObject;

            if (GUILayout.Button("move_object"))
            {
                if (moveObject != null)
                {
                    Vector3 pos = moveObject.transform.position;
                    cme.move_object(targetID, pos.x, pos.y, pos.z);
                }
                else
                {
                    cme.move_object(targetID, moveTarget.x, moveTarget.y, moveTarget.z);
                }
            }

            //----------------------------------------
            // 3. 회전
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Rotate Settings", EditorStyles.boldLabel);
            rotateAngle = EditorGUILayout.FloatField("Angle", rotateAngle);

            if (GUILayout.Button("rotate_object"))
            {
                cme.rotate_object(targetID, rotateAngle);
            }

            //----------------------------------------
            // 4. 스케일
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Scale Settings", EditorStyles.boldLabel);
            scaleFactor = EditorGUILayout.FloatField("Scale Factor", scaleFactor);

            if (GUILayout.Button("scale_object"))
            {
                cme.scale_object(targetID, scaleFactor);
            }

            //----------------------------------------
            // 5. Appearance (Color / Texture)
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Appearance Settings", EditorStyles.boldLabel);

            colorHex = EditorGUILayout.TextField("Color (Hex)", colorHex);
            if (GUILayout.Button("set_color"))
            {
                cme.set_color(targetID, colorHex);
            }

            textureName = EditorGUILayout.TextField("Texture Name", textureName);
            if (GUILayout.Button("set_texture"))
            {
                cme.set_texture(targetID, textureName);
            }

            //----------------------------------------
            // 6. Physics
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);

            weight = EditorGUILayout.FloatField("Weight", weight);
            if (GUILayout.Button("set_weight"))
            {
                cme.set_weight(targetID, weight);
            }

            //----------------------------------------
            // 7. Remove Object
            //----------------------------------------
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Remove Object", EditorStyles.boldLabel);

            if (GUILayout.Button("remove_object"))
            {
                cme.remove_object(targetID);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 활성화됩니다.", MessageType.Info);
        }
    }
}

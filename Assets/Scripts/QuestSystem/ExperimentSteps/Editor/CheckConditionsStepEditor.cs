#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckConditionsStep))]
[CanEditMultipleObjects]
public class CheckConditionsStepEditor : Editor
{
    // 직렬화된 속성(SerializedProperty) 가져오기
    SerializedProperty currentMode;
    SerializedProperty checkInterval;
    SerializedProperty groupA;
    SerializedProperty groupB;
    SerializedProperty geometricTolerance;

    // 부모 클래스(WaitEnterA 등)의 변수들도 그리기 위해 필요
    // (만약 부모 변수가 안 보인다면 이 부분은 생략 가능하지만, 안전하게 기본 인스펙터도 호출합니다)

    private void OnEnable()
    {
        // 변수 연결
        currentMode = serializedObject.FindProperty("currentMode");
        checkInterval = serializedObject.FindProperty("checkInterval");
        groupA = serializedObject.FindProperty("groupA");
        groupB = serializedObject.FindProperty("groupB");
        geometricTolerance = serializedObject.FindProperty("geometricTolerance");
    }

    public override void OnInspectorGUI()
    {
        // 1. 업데이트 시작
        serializedObject.Update();

        // 2. 기본적으로 그려져야 할 것들 (부모 클래스 변수 + 스크립트 참조 필드 등)
        // DrawDefaultInspector(); // 이걸 쓰면 숨긴 것 빼고 다 그리지만, 순서 제어를 위해 직접 그립니다.
        
        // 스크립트 파일 참조 필드 (유니티 기본 스타일)
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoBehaviour), false);
        }

        // 부모 클래스(WaitEnterA)의 필드들을 먼저 그립니다.
        // (WaitScenarioTask에 없는 필드만 찾아서 그리는 로직)
        DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "currentMode", "checkInterval", "groupA", "groupB", "geometricTolerance" });

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scenario Settings", EditorStyles.boldLabel);

        // 3. 공통 설정 그리기
        EditorGUILayout.PropertyField(currentMode);
        EditorGUILayout.PropertyField(checkInterval);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Condition Details", EditorStyles.boldLabel);

        // 4. 모드에 따른 조건부 그리기 (핵심!)
        CheckConditionsStep.TaskMode mode = (CheckConditionsStep.TaskMode)currentMode.enumValueIndex;

        switch (mode)
        {
            case CheckConditionsStep.TaskMode.CheckAttributes:
                // 속성 검사 모드: Group A만 필요
                EditorGUILayout.PropertyField(groupA, new GUIContent("Target Objects (검사 대상)"));
                EditorGUILayout.HelpBox("설정된 속성(Color, Scale 등)과 개수를 만족하는지 검사합니다.", MessageType.Info);
                break;

            case CheckConditionsStep.TaskMode.CheckProximity:
                // 근접 검사 모드: A, B, 거리 필요
                EditorGUILayout.PropertyField(geometricTolerance, new GUIContent("Max Distance (최대 거리)"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(groupA, new GUIContent("Group A (기준 대상)"));
                EditorGUILayout.PropertyField(groupB, new GUIContent("Group B (이동 대상)"));
                EditorGUILayout.HelpBox("Group A와 Group B 사이의 거리가 위 값 이내인지 검사합니다.", MessageType.Info);
                break;

            case CheckConditionsStep.TaskMode.CheckLinear:
                // 일렬 정렬: A, 오차범위 필요
                EditorGUILayout.PropertyField(geometricTolerance, new GUIContent("Line Tolerance (직선 오차)"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(groupA, new GUIContent("Objects to Align (정렬 대상)"));
                EditorGUILayout.HelpBox("대상들이 일직선상에 위치하는지 검사합니다.", MessageType.Info);
                break;

            case CheckConditionsStep.TaskMode.CheckCircular:
                // 원형 정렬: A, 오차범위 필요
                EditorGUILayout.PropertyField(geometricTolerance, new GUIContent("Radius Tolerance (반지름 오차)"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(groupA, new GUIContent("Objects in Circle (원형 배치 대상)"));
                EditorGUILayout.HelpBox("대상들이 원형으로 배치되었는지(중심으로부터 거리가 일정) 검사합니다.", MessageType.Info);
                break;
        }

        // 5. 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
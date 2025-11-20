// File: LookObjectSelector.cs
using UnityEngine;

public class LookObjectSelector : MonoBehaviour
{
    [Header("===== SelectObject Parameters =====")]
    public float maxDistance = 50f;
    public float viewportRadius = 0.1f; // 화면 중앙 기준 반경 (0~0.5)
    public LayerMask interactableLayers;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("메인 카메라를 찾을 수 없습니다!");
        }
    }
    /*디버그용
    private void Update()
    {
        SelectObject();
    }
    */

    
    public string SelectObject()
    {
        if (cam == null)
        {
            Debug.LogError("카메라가 초기화되지 않았습니다!");
            return null;
        }

        Collider[] cols = Physics.OverlapSphere(cam.transform.position, maxDistance, interactableLayers);
        GameObject target = null;
        float bestScreenDist = float.MaxValue;
        float bestDistSq = float.MaxValue;

        foreach (var col in cols)
        {
            Vector3 vp = cam.WorldToViewportPoint(col.transform.position);
            if (vp.z <= 0f) // 카메라 뒤쪽에 있는 경우 제외
                continue;

            float dx = vp.x - 0.5f;
            float dy = vp.y - 0.5f;
            float screenDist = Mathf.Sqrt(dx * dx + dy * dy);
            if (screenDist > viewportRadius)
                continue;

            float distSq = (col.transform.position - cam.transform.position).sqrMagnitude;
            if (screenDist < bestScreenDist
                || (Mathf.Approximately(screenDist, bestScreenDist) && distSq < bestDistSq))
            {
                bestScreenDist = screenDist;
                bestDistSq = distSq;
                target = col.gameObject;
            }
        }

        if (target == null)
        {
            Debug.Log("선택된 오브젝트가 없습니다.");
            return null;
        }

        var furnitureRef = target.GetComponent<EntityInfoAgent>();
        if (furnitureRef == null)
        {
            Debug.LogError("선택된 오브젝트에 FurnitureReference 컴포넌트가 없습니다!");
            return null;
        }

        string result = furnitureRef.Info.type;

        Debug.Log("이거 or 저거 : " + result);
        return result;
    }
    public string ChangeResult(string command)
    {
        string result = SelectObject();
        return ReplaceCommand(command, result);
    }
    public static string ReplaceCommand(string command, string replacement)
    {
        if (string.IsNullOrEmpty(command) || string.IsNullOrEmpty(replacement))
            return command;

        bool containsTarget = command.Contains("목표");

        if (!containsTarget)
            return command;

        // "목표"를 replacement로 교체
        string result = command.Replace("목표", replacement);

        return result;
    }
}

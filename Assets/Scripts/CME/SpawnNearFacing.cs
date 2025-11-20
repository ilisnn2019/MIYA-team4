using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnNearFacing : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject testPrefab;      // 테스트용 프리팹
    public LayerMask surfaceMask;
    public LayerMask objectMask;

    [Header("Gizmo Debug")]
    public bool drawGizmos = true;
    public Color gizmoStartColor = Color.yellow;
    public Color gizmoStepColor = new Color(0f, 0.6f, 1f);
    public Color gizmoFinalColor = Color.green;
    public float gizmoSphereRadius = 0.1f;

    // 내부 저장용 (Gizmo 경로)
    private List<Vector3> debugPositions = new List<Vector3>();


    // ===============================
    //  메인 기능
    // ===============================
    public Vector3 FindSafeSpawnPosition(GameObject prefab, Vector3 initialPosition)
    {
        debugPositions.Clear();
        debugPositions.Add(initialPosition);

        Collider prefabCollider = prefab.GetComponent<Collider>();
        if (prefabCollider == null)
        {
            Debug.LogWarning($"[FindSafeSpawnPosition] Prefab '{prefab.name}'에 Collider가 필요합니다.");
            return initialPosition;
        }

        float radius = GetObjectRadius(prefab);
        float stepDistance = Mathf.Max(radius * 0.5f, 0.05f);
        int maxIterations = 20;

        Vector3 currentPosition = initialPosition;

        // DOWN Ray로 지면 히트 여부 확인
        bool hitGround = Physics.Raycast(initialPosition + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 5f, surfaceMask, QueryTriggerInteraction.Ignore);

        if (!hitGround)
        {
            // 공중이라면 -> Terrain 기반 보정 수행
            currentPosition = FindGroundSurfacePoint(initialPosition, surfaceMask, prefab);
        }
        // else: 이미 지면 위면 그대로 둠

        debugPositions.Add(currentPosition);

        // 이후 기존의 충돌 회피 루프
        for (int i = 0; i < maxIterations; i++)
        {
            Collider[] overlaps = Physics.OverlapSphere(currentPosition, radius, objectMask, QueryTriggerInteraction.Ignore);
            if (overlaps.Length == 0)
                break;

            Vector3 totalMoveDir = Vector3.zero;
            int validHits = 0;

            foreach (Collider hitCol in overlaps)
            {
                if (hitCol == null || hitCol == prefabCollider || hitCol.gameObject == prefab)
                    continue;

                if (Physics.ComputePenetration(
                    hitCol, hitCol.transform.position, hitCol.transform.rotation,
                    prefabCollider, currentPosition, prefab.transform.rotation,
                    out Vector3 direction, out float distance))
                {
                    validHits++;
                    float upDot = Vector3.Dot(direction, Vector3.up);
                    if (upDot > 0.7f)
                        totalMoveDir += Vector3.up * (1f - upDot);
                    else
                        totalMoveDir += direction;
                }
            }

            if (validHits == 0 || totalMoveDir == Vector3.zero)
                break;

            totalMoveDir.Normalize();
            currentPosition += totalMoveDir * stepDistance;
            debugPositions.Add(currentPosition);

            if (Physics.OverlapSphere(currentPosition, radius * 0.95f, objectMask, QueryTriggerInteraction.Ignore).Length == 0)
                break;
        }

        debugPositions.Add(currentPosition);
        return currentPosition;
    }

    private float GetObjectRadius(GameObject prefab)
    {
        float maxSize = 1f;

        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            maxSize = Mathf.Max(1f, Mathf.Max(size.x, Mathf.Max(size.y, size.z)));
        }
        else
        {
            Collider col = prefab.GetComponentInChildren<Collider>();
            if (col != null)
            {
                Vector3 size = col.bounds.size;
                maxSize = Mathf.Max(1f, Mathf.Max(size.x, Mathf.Max(size.y, size.z)));
            }
        }

        return maxSize * 0.5f;
    }

    private Vector3 FindGroundSurfacePoint(Vector3 startPos, LayerMask surfaceMask, GameObject prefab)
    {
        // Terrain 확인
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogWarning("[FindGroundSurfacePoint] Scene에 Terrain이 없습니다.");
            return startPos;
        }

        //  Terrain 높이 샘플
        float terrainHeight = terrain.SampleHeight(startPos);

        // prefab의 메시 크기 가져오기 (y축 절반)
        float halfHeight = 0.5f; // 기본값
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            halfHeight = renderer.bounds.size.y;
        }
        else
        {
            Collider col = prefab.GetComponentInChildren<Collider>();
            if (col != null)
                halfHeight = col.bounds.size.y;
        }

        // 최종 위치 계산
        Vector3 result = new Vector3(startPos.x, terrainHeight + halfHeight, startPos.z);

#if UNITY_EDITOR
        Debug.DrawLine(startPos, result, Color.cyan, 2f); // Scene 뷰 디버그
#endif

        return result;
    }


    // ===============================
    // ContextMenu 기능 (에디터 전용)
    // ===============================
#if UNITY_EDITOR
    [ContextMenu("테스트 스폰 위치 미리보기")]
    private void PreviewSpawnPosition()
    {
        surfaceMask = 1 << LayerMask.NameToLayer("Surface");
        objectMask = 1 << (LayerMask.NameToLayer("Grabbable") | LayerMask.NameToLayer("Grabbing"));
        if (testPrefab == null)
        {
            Debug.LogWarning("[SpawnNearFacing] 테스트용 Prefab이 지정되지 않았습니다.");
            return;
        }

        Vector3 result = FindSafeSpawnPosition(testPrefab, testPrefab.transform.position);
        Debug.Log($"[SpawnNearFacing] 테스트 스폰 결과 위치: {result}");
        UnityEditor.SceneView.RepaintAll(); // Scene 뷰 갱신
    }


    // ===============================
    //  Gizmo 표시 (Scene View)
    // ===============================
    private void OnDrawGizmos()
    {
        if (!drawGizmos || debugPositions == null || debugPositions.Count == 0)
            return;

        // 경로 시각화
        for (int i = 0; i < debugPositions.Count - 1; i++)
        {
            Gizmos.color = Color.Lerp(gizmoStartColor, gizmoStepColor, (float)i / debugPositions.Count);
            Gizmos.DrawSphere(debugPositions[i], gizmoSphereRadius);
            Gizmos.DrawLine(debugPositions[i], debugPositions[i + 1]);
        }

        // 마지막 위치 강조
        Gizmos.color = gizmoFinalColor;
        Gizmos.DrawSphere(debugPositions[debugPositions.Count - 1], gizmoSphereRadius * 1.5f);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Check Surface Hit")]
    private void CheckSurfaceHit()
    {
        float maxDistance = 50;

        if (testPrefab == null)
        {
            Debug.LogWarning("Test Prefab이 지정되지 않았습니다.");
            return;
        }

        Vector3 pos = testPrefab.transform.position;

        // 위쪽 Ray
        bool hitUp = Physics.Raycast(pos, Vector3.up, out RaycastHit upHit, maxDistance, surfaceMask);
        // 아래쪽 Ray
        bool hitDown = Physics.Raycast(pos, Vector3.down, out RaycastHit downHit, maxDistance, surfaceMask);

        // 결과 출력
        string result = $"Surface Check at {pos}:\n" +
                        $"  Up Hit: {hitUp}" + (hitUp ? $" at {upHit.point}" : "") + "\n" +
                        $"  Down Hit: {hitDown}" + (hitDown ? $" at {downHit.point}" : "");

        Debug.Log(result);

        // Scene 뷰 시각화
        Debug.DrawRay(pos, Vector3.up * maxDistance, hitUp ? Color.green : Color.red, 2f);
        Debug.DrawRay(pos, Vector3.down * maxDistance, hitDown ? Color.green : Color.red, 2f);
    }

    [ContextMenu("Check height Sampling")]
    public void CheckSampleHeight()
    {
        Debug.Log(Terrain.activeTerrain.SampleHeight(testPrefab.transform.position));
    }
#endif


}

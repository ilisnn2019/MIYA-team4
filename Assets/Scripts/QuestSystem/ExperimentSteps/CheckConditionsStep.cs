using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 효율적인 리스트 캐싱을 통해 씬 상태를 검사하는 퀘스트 스텝
/// </summary>
public class CheckConditionsStep : WaitEnterA
{
    public enum TaskMode
    {
        CheckAttributes,    // 생성, 색상, 크기, 무게 등 속성 검사
        CheckProximity,     // 두 그룹 간의 거리 검사
        CheckLinear,        // 일렬 정렬
        CheckCircular       // 원형 정렬
    }

    [Header("Task Configuration")]
    public TaskMode currentMode = TaskMode.CheckAttributes;

    [Tooltip("완료에 대한 검사 주기 (초 단위). 만약 실험시간에 민감하면 더 줄여야됨")]
    public float checkInterval = 0.2f;
    private float _timer = 0f;

    // ---------------------------------------------------------
    // 캐싱된 에이전트 리스트 (핵심 최적화)
    // ---------------------------------------------------------
    [SerializeField]private List<EntityInfoAgent> _cachedAgents = new List<EntityInfoAgent>();

    // ---------------------------------------------------------
    // 조건 정의 (Inspector용)
    // ---------------------------------------------------------
    [System.Serializable]
    public class ObjectCriteria
    {
        public string label = "조건 설명";

        [Header("Filter")]
        [Tooltip("객체 이름/타입 필터 (빈칸이면 모든 객체 대상)")]
        public string nameContains;
        [Tooltip("필요 최소 개수")]
        public int requiredCount = 1;

        [Header("Common Attributes (공통 속성)")]
        public bool checkColor;
        public string targetColor;
        
        public bool checkTexture;
        public string targetTexture;

        public bool checkScale;
        public float targetScale;
        public float scaleTolerance = 0.1f;

        public bool checkRotation;
        public float targetRotation;
        public float rotationTolerance = 5f;

        public bool checkWeight;
        public float targetWeight;
        public float weightTolerance = 0.1f;

        [Header("Complex Conditions (복합 조건)")]
        [Tooltip("체크하면 단일 TargetColor 대신 아래 리스트의 색상들이 모두 포함되어 있는지 검사합니다.")]
        public bool checkDistinctColors; 
        [Tooltip("반드시 포함되어야 하는 색상 목록 (예: Red, Green)")]
        public List<string> distinctColors;
    }

    public ObjectCriteria groupA; // 주 대상
    public ObjectCriteria groupB; // 비교 대상 (Proximity 모드용)
    public float geometricTolerance = 0.5f; // 거리/정렬 오차 범위


    // ---------------------------------------------------------
    // 1. 초기화 및 이벤트 구독 (최적화 핵심)
    // ---------------------------------------------------------
    protected override void OnEnable()
    {
        base.OnEnable();

        // 1) 초기 스캔: 이미 씬에 있는 객체들 등록
        _cachedAgents.Clear();
        _cachedAgents.AddRange(FindObjectsByType<EntityInfoAgent>(FindObjectsSortMode.None));

        // 2) 이벤트 구독: 앞으로 생성될 객체들 등록
        if (EventsManager.instance != null)
        {
            EventsManager.instance.conditionEvents.OnSpawnedObject += OnObjectSpawned;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // 이벤트 구독 해제 (메모리 누수 방지)
        if (EventsManager.instance != null)
        {
            EventsManager.instance.conditionEvents.OnSpawnedObject -= OnObjectSpawned;
        }
        _cachedAgents.Clear();
    }

    // 이벤트 핸들러: 새 객체가 생성되면 리스트에 추가
    private void OnObjectSpawned(EntityInfoAgent newAgent)
    {
        if (newAgent != null && !_cachedAgents.Contains(newAgent))
        {
            _cachedAgents.Add(newAgent);
            Debug.Log($"[WaitScenarioTask] 리스트에 추가됨: {newAgent.name}");
        }
    }

    // ---------------------------------------------------------
    // 2. Update 루프 (검사 로직)
    // ---------------------------------------------------------
    protected override void Update()
    {
        base.Update();

        _cachedAgents.RemoveAll(a => a == null);

        bool passed = false;

        // 1) 모드에 따른 검사 수행
        switch (currentMode)
        {
            case TaskMode.CheckAttributes:
                passed = EvaluateAttributes(_cachedAgents, groupA);
                break;
            case TaskMode.CheckProximity:
                passed = EvaluateProximity(_cachedAgents, groupA, groupB, geometricTolerance);
                break;
            case TaskMode.CheckLinear:
                passed = EvaluateLinear(_cachedAgents, groupA, geometricTolerance);
                break;
            case TaskMode.CheckCircular:
                passed = EvaluateCircular(_cachedAgents, groupA, geometricTolerance);
                break;
        }

        if (passed)
        {
            FinishQuestStep();
        }
    }

    // ---------------------------------------------------------
    // 3. 평가 로직 (Evaluation Methods)
    // ---------------------------------------------------------

    private bool EvaluateAttributes(List<EntityInfoAgent> pool, ObjectCriteria criteria)
    {
        var matches = FilterAgents(pool, criteria);
        
        // 속성 검사에서도 복합 색상 조건을 사용할 수 있도록 추가
        if (criteria.checkDistinctColors)
        {
            if (!CheckColorDiversity(matches, criteria.distinctColors)) return false;
        }

        return matches.Count >= criteria.requiredCount;
    }

    private bool EvaluateProximity(List<EntityInfoAgent> pool, ObjectCriteria criteriaA, ObjectCriteria criteriaB, float distLimit)
    {
        var listA = FilterAgents(pool, criteriaA);
        var listB = FilterAgents(pool, criteriaB);

        if (listA.Count == 0 || listB.Count == 0) return false;

        foreach (var a in listA)
        {
            foreach (var b in listB)
            {
                if (a == b) continue;
                if (Vector3.Distance(a.Info.position, b.Info.position) <= distLimit)
                    return true;
            }
        }
        return false;
    }

    private bool EvaluateLinear(List<EntityInfoAgent> pool, ObjectCriteria criteria, float tolerance)
    {
        // 1. 필터링 (여기서는 색상을 제외하고 모양/이름 등으로만 1차 필터링 권장)
        var targets = FilterAgents(pool, criteria);

        // 2. 개수 확인
        if (targets.Count < criteria.requiredCount) return false;
        if (targets.Count < 2) return true; // 점 1개는 논외

        // 3. 정렬 확인 (X축 기준 정렬)
        var sortedList = targets.OrderBy(t => t.Info.position.x).ToList();
        Vector3 start = sortedList[0].Info.position;
        Vector3 end = sortedList[sortedList.Count - 1].Info.position;
        Vector3 lineDir = (end - start).normalized;

        for (int i = 1; i < sortedList.Count - 1; i++)
        {
            Vector3 targetPos = sortedList[i].Info.position;
            Vector3 projected = start + Vector3.Project(targetPos - start, lineDir);
            if (Vector3.Distance(targetPos, projected) > tolerance) return false;
        }

        // 4. [추가됨] 복합 색상 조건 확인 (빨강, 초록이 다 있나?)
        if (criteria.checkDistinctColors)
        {
            if (!CheckColorDiversity(targets, criteria.distinctColors)) return false;
        }

        return true;
    }

    private bool EvaluateCircular(List<EntityInfoAgent> pool, ObjectCriteria criteria, float tolerance)
    {
        var targets = FilterAgents(pool, criteria);
        if (targets.Count < criteria.requiredCount) return false;
        if (targets.Count < 3) return false; 

        // 1. 무게 중심 및 반지름 계산
        Vector3 center = Vector3.zero;
        foreach (var t in targets) center += t.Info.position;
        center /= targets.Count;

        float avgRadius = 0f;
        foreach (var t in targets) avgRadius += Vector3.Distance(center, t.Info.position);
        avgRadius /= targets.Count;

        // 2. 원 궤도 이탈 확인
        foreach (var t in targets)
        {
            float r = Vector3.Distance(center, t.Info.position);
            if (Mathf.Abs(r - avgRadius) > tolerance) return false;
        }

        // 3. [추가됨] 복합 색상 조건 확인 (빨,초,파,노 다 있나?)
        if (criteria.checkDistinctColors)
        {
            if (!CheckColorDiversity(targets, criteria.distinctColors)) return false;
        }

        return true;
    }

    // ---------------------------------------------------------
    // 4. 필터링 헬퍼 (EntityInfo 데이터 기반 비교)
    // ---------------------------------------------------------
    private List<EntityInfoAgent> FilterAgents(List<EntityInfoAgent> pool, ObjectCriteria criteria)
    {
        List<EntityInfoAgent> result = new List<EntityInfoAgent>();

        foreach (var agent in pool)
        {
            // pool은 Update 초입에서 null 체크를 했으므로 여기선 안전함
            
            // 1) 이름 필터
            if (!string.IsNullOrEmpty(criteria.nameContains))
            {
                // CommandExecutor가 이름을 "Type_ID"로 지으므로 이름에 Type이 포함됨
                if (agent.name.IndexOf(criteria.nameContains, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
            }
            // checkcolor와 공존 안됨
            if (!criteria.checkDistinctColors && criteria.checkColor)
            {
                if (string.IsNullOrEmpty(agent.Info.color) || 
                    !agent.Info.color.Equals(criteria.targetColor, System.StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            // 2) 속성 필터 (EntityInfo 값 사용)
            if (criteria.checkColor)
            {
                if (string.IsNullOrEmpty(agent.Info.color) || 
                    !agent.Info.color.Equals(criteria.targetColor, System.StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            if (criteria.checkTexture)
            {
                if (string.IsNullOrEmpty(agent.Info.texture) || 
                    !agent.Info.texture.Equals(criteria.targetTexture, System.StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            if (criteria.checkScale)
            {
                if (Mathf.Abs(agent.Info.factor - criteria.targetScale) > criteria.scaleTolerance)
                    continue;
            }

            if (criteria.checkRotation)
            {
                float diff = Mathf.Abs(Mathf.DeltaAngle(agent.Info.rotation, criteria.targetRotation));
                if (diff > criteria.rotationTolerance)
                    continue;
            }

            if (criteria.checkWeight)
            {
                if (Mathf.Abs(agent.Info.weight - criteria.targetWeight) > criteria.weightTolerance)
                    continue;
            }

            result.Add(agent);
        }
        return result;
    }

    private bool CheckColorDiversity(List<EntityInfoAgent> agents, List<string> requiredColors)
    {
        if (requiredColors == null || requiredColors.Count == 0) return true;

        // 필요한 색상 중 하나라도 찾지 못하면 false 반환
        foreach (var reqColor in requiredColors)
        {
            bool found = false;
            foreach (var agent in agents)
            {
                if (agent.Info.color != null && 
                    agent.Info.color.Equals(reqColor, System.StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false; // 이 색상을 가진 객체가 없음
        }

        return true;
    }
}
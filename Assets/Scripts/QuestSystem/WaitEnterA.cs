using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class WaitEnterA : QuestStep
{
    public UnityEvent OnStepNarrationDone;
    [SerializeField]
    private float time = 3f;
    [SerializeField]
    private float alphaTime = 0f;
    
    // [수정] private -> protected: 자식 클래스가 타이머 상태를 알 수 있도록 변경
    protected bool waitTimer = true;
    
    Coroutine timerCoroutine;
    
    // [수정] private -> protected: 자식 클래스가 접근할 수 있도록 변경
    protected bool isAPressedLastFrame = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        OnStepNarrationDone.AddListener(() =>
        {
            if (quest.enterUI != null)
                quest.enterUI.SetActive(true);
        });

        OnStepFinished.AddListener(() =>
        {
            if (quest.enterUI != null)
                quest.enterUI.SetActive(false);
        });

        if (OnStartNarration != null)
        {
            time = OnStartNarration.length + alphaTime;
            waitTimer = true;
        }
        timerCoroutine = StartCoroutine(StartTimer());
        isAPressedLastFrame = false;
    }

    protected virtual void Update()
    {
        // 내레이션(타이머) 재생 중이면 입력 무시
        if (waitTimer) return;

        // 1. 키보드 스페이스바 입력 (테스트용)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinishQuestStep();
            return;
        }

        // 2. 메타 퀘스트 'A' 버튼 직접 확인
        CheckAButtonInput();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnStepNarrationDone.RemoveAllListeners();
        
        if(timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    // [수정] private -> protected: 자식 클래스에서 필요시 사용할 수 있도록 변경
    protected IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(time);
        waitTimer = false;
        OnStepNarrationDone.Invoke();
    }

    // [수정] private -> protected virtual: 자식 클래스에서 이 로직을 재정의(override)할 수 있도록 함
    protected virtual void CheckAButtonInput()
    {
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHandDevice.isValid)
        {
            bool isAPressedThisFrame = false;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isAPressedThisFrame))
            {
                if (isAPressedThisFrame && !isAPressedLastFrame)
                {
                    FinishQuestStep();
                }
                isAPressedLastFrame = isAPressedThisFrame;
            }
            else
            {
                isAPressedLastFrame = false;
            }
        }
        else
        {
            isAPressedLastFrame = false;
        }
    }
}
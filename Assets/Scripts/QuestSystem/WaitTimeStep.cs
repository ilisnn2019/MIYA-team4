using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTimeStep : QuestStep
{
    [SerializeField]
    private float time = 5f; //���ð�
    [SerializeField]
    private float alphaTime = 0f; //�߰����ð�

    Coroutine timerCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        if(OnStartNarration != null)
        {
            time = OnStartNarration.length + alphaTime ;

        }
        timerCoroutine = StartCoroutine(StartTimer());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopCoroutine(timerCoroutine);
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(time);
        FinishQuestStep();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//QuestStep ��ũ��Ʈ�� �ۼ��� �� �� Ŭ������ ��� �޾Ƽ� �ۼ�
public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false; //���� �Ϸ� ����
    public Quest quest; //���� ����Ʈ
    public string text = "";
    //public int stepIndex; //������ �ε���

    [Header("설정할 값 (null이면 무시됨)")]
    [SerializeField] public AudioClip OnStartNarration;
    public GameObject uiImage;

    public UnityEvent OnStepStart;
    public UnityEvent OnStepFinished;

    protected virtual void OnEnable()
    {
        OnStepStart.AddListener(EnableUI);
        OnStepFinished.AddListener(DisableUI);
        //Debug.Log("on");
    }

    protected virtual void OnDisable()
    {
        OnStepStart.RemoveAllListeners();
        OnStepFinished.RemoveAllListeners();
        //Debug.Log("off");
    }
    
    //�ʱ�ȭ
    public void InitializeQuestStep(Quest quest)
    {
        isFinished = false;
        this.quest = quest;
        //this.stepIndex = stepIndex;
    }

    //���� �Ϸ�
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            EventsManager.instance.questEvents.AdvanceQuest(quest);
            //OnStepFinished.Invoke();
            //Destroy(this.gameObject);
            gameObject.SetActive(false);
        }
    }

    private void EnableUI()
    {
        if(uiImage != null)
        {
            uiImage.SetActive(true);
        }
    }

    private void DisableUI()
    {
        if(uiImage != null)
        {
            uiImage.SetActive(false);
        }
    }
}

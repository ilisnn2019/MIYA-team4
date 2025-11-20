using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [Header("Quests")]
    public Quest[] questList;
    public int currentQuestIndex = 0;

    //[Header("UI")]
    //public QuestUI ui;

    [Header("Awake")]
    [Tooltip("�����Ҷ� ����Ʈ�� �ڵ����� �����ų��")]
    public bool startFirstQuestOnAwake = true;

    //�ʱ�ȭ
    private void Awake()
    {
        foreach (Quest quest in questList)
        {
            quest.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // 자식 오브젝트들에서 Quest 컴포넌트를 가진 것들을 모두 가져옴
        questList = GetComponentsInChildren<Quest>(true);
        
        // 모든 퀘스트를 비활성화 상태로 시작
        foreach (Quest quest in questList)
        {
            quest.gameObject.SetActive(false);
            quest.state = QuestState.CAN_START;
        }

         if (questList.Length > 0 && startFirstQuestOnAwake)
        {
            StartQuest(0);
        }
        else
        {
            currentQuestIndex = -1;
        }

        if (EventsManager.instance != null)
        {
            EventsManager.instance.questEvents.onStartQuest += StartQuest;
            EventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
            EventsManager.instance.questEvents.onFinishQuest += FinishQuest;
        }
        else
        {
            Debug.LogError("EventsManager instance is NULL in Start(). Check Project Settings -> Script Execution Order.");
        }
    }
    private void OnDisable()
    {
        //�̺�Ʈ ���� ����
        EventsManager.instance.questEvents.onStartQuest -= StartQuest;
        EventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        EventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
    }

    //����Ʈ ����
    private void StartQuest(int index)
    {
        currentQuestIndex = index;
        if (questList[index].state != QuestState.CAN_START) //state Ȯ��  
        {
            //TODO : �ѹ� ������ ������ �ٽ� �� �� ������ ���� ó��

        }
        //ui.Init(questList[index]); //UI ����
        for (int i = 0; i < questList.Length; i++)
            if (i != index)
                questList[i].gameObject.SetActive(false);

        questList[index].gameObject.SetActive(true);
        questList[index].state = QuestState.IN_PROGRESS;
    }

    //����Ʈ ���� ����
    private void AdvanceQuest(Quest quest)
    {
        quest.MoveToNextStep(); //���� Step���� �̵�
    }

    //����Ʈ �Ϸ�
    private void FinishQuest(Quest quest)
    {
        quest.state = QuestState.FINISHED;
        //SceneManager.LoadScene("Intro");
        quest.gameObject.SetActive(false); //����� ����Ʈ disable
    }

}

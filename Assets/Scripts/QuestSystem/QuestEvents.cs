using System;

public class QuestEvents
{
    public event Action<int> onStartQuest; //����Ʈ ���� �̺�Ʈ
    public void StartQuest(int index)
    {
        if (onStartQuest != null)
        {
            onStartQuest(index);
        }
    }

    public event Action<Quest> onAdvanceQuest; //����Ʈ Step ���� �̺�Ʈ
    public void AdvanceQuest(Quest quest)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(quest);
        }
    }

    public event Action<Quest> onFinishQuest; //����Ʈ ���� �̺�Ʈ
    public void FinishQuest(Quest quest)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(quest);
        }
    }

    public event Action<Quest> onQuestStateChange; //����Ʈ ���� ���� �̺�Ʈ
    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }
}

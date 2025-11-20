using UnityEngine;
using UnityEngine.SceneManagement;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance { get; private set; }

    public QuestEvents questEvents; //퀘스트 진행상황
    public ConditionEvents conditionEvents; //조건 충족 상황
    public static int SceneNumber = 0;
    public GameObject player;  //플레이어 설정은 해당 오브젝트로 참조

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one EventsManager in the scene.");
            Destroy(gameObject); 
            return;
        }
        instance = this;
        player = GameObject.FindWithTag("Player");

        DontDestroyOnLoad(gameObject);
        
        questEvents = new QuestEvents();
        conditionEvents = new ConditionEvents();
    }
}
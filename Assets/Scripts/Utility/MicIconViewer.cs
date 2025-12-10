using UnityEngine;
using UnityEngine.UI;

public class MicIconViewer : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Image i_image;
    [SerializeField] private Sprite i_microphone;
    [SerializeField] private Sprite i_thinking;
    [SerializeField] private Sprite i_check;

    [Header("Volume Levels")]
    [SerializeField] private Image v_image;
    [SerializeField] private Sprite[] volumeSprites;

    private const float SILENCE_THRESHOLD = 0.01f;

    private bool isListening = false;


    private void Start()
    {

    }

    private int prevLevel = -1;

    private void Update()
    {
        if (!isListening)
        {
            SetVolumeSprite(0);
            return;
        }

        float volume = VoiceRecorder.CurrentVolume;

        // SILENCE_THRESHOLD 이하이면 항상 0
        if (volume <= SILENCE_THRESHOLD)
        {
            SetVolumeSprite(0);
            return;
        }

        // 임계점 제거 후 단계 계산
        float adjusted = volume - SILENCE_THRESHOLD;
        int level = Mathf.FloorToInt(adjusted / SILENCE_THRESHOLD) + 1; // 1~?

        // 1~3으로 제한
        level = Mathf.Clamp(level, 1, 3);

        SetVolumeSprite(level);
    }

    private void SetVolumeSprite(int level)
    {
        if (prevLevel == level) return;
        v_image.sprite = volumeSprites[level];
        prevLevel = level;
    }

    public void StartListening()
    {
        i_image.sprite = i_microphone;
        i_image.color = Color.white;
    }

    public void DetectingWakeWord()
    {
        i_image.color = Color.green;
        isListening = true;
    }

    public void ThinkLLM()
    {
        isListening = false;
        i_image.sprite = i_thinking;
        i_image.color = Color.white;
    }

    public void EndCommand()
    {
        isListening = false;
        i_image.sprite = i_check;
        i_image.color = Color.white;
    }
}

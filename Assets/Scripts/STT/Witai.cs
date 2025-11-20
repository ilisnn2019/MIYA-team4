using Meta.WitAi;
using UnityEngine;

namespace WakeWordModule
{
    public class Witai
    {
        [SerializeField] private VoiceService voiceService;

        public void OnDetectWakeWord(string[] entityValues)
        {
            Debug.Log(entityValues[0]);
            Debug.Log("WakeUp");
        }
    }
}
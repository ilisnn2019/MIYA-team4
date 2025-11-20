using System;
using Meta.WitAi.Events;
using UnityEngine;

public enum VoiceState
{
    Idle,
    Listening,
    ProcessingSTT,
    ProcessingChat
}

public interface IStateEventSubscriber
{
    void RegisterToStateManager(Action call);
}

public class VoiceFlowManager : MonoBehaviour
{ }

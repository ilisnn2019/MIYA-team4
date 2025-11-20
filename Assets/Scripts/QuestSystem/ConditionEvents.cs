using System;
using UnityEngine;

public class ConditionEvents
{
    public event Action<EntityInfoAgent> OnSpawnedObject;
    public void SpawnedObject(EntityInfoAgent entity)
    {
        OnSpawnedObject?.Invoke(entity);
    }
}

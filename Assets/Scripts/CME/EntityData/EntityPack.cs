using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntitySpec
{
    public string entityType;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "EntityPack", menuName = "Game/Entity Pack", order = 100)]
public class EntityPack : ScriptableObject
{
    public List<EntitySpec> entities;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntitySpec", menuName = "Game/Entity Spec", order = 100)]
public class EntitySpec : ScriptableObject
{
    [Header("Reference Prefab")]
    public GameObject prefab;

    [Header("Entity Basic Info")]
    public string entityType = "";
}


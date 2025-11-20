
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityPack", menuName = "Game/Entity Pack", order = 101)]
public class EntityPack : ScriptableObject
{
    [Header("Entity Spec")]
    public List<EntitySpec> entitySpec;
}
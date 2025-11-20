using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "Furniture/Furniture Data", order = 0)]
public class FurnitureData : ScriptableObject
{
    public string type;
    public Vector3 size;
}

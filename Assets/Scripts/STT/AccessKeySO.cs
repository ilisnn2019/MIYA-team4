using UnityEngine;

[CreateAssetMenu(fileName = "AccessKey", menuName = "NLP/AccessKey", order = 1)]
public class AccessKeySO : ScriptableObject
{
    [Tooltip("Enter your Access Key here.")]
    public string accessKey;
}
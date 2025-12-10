using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureSpec
{
    public string identifier = "";
    public Texture texture;
}

[CreateAssetMenu(fileName = "TexturePack", menuName = "Game/Texture Pack", order = 103)]
public class TexturePack : ScriptableObject
{
    [Header("Texture Spec")]
    public List<TextureSpec> textureSpec;
}
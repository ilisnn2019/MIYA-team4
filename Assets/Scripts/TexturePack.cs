using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureSpec", menuName = "Game/Texture Spec", order = 102)]
public class TextureSpec : ScriptableObject
{
    [Header("Reference Prefab")]
    public Texture texture;
    [Header("Texture Basic Info")]
    public string identifier = "";
}

[CreateAssetMenu(fileName = "TexturePack", menuName = "Game/Texture Pack", order = 103)]
public class TexturePack : ScriptableObject
{
    [Header("Texture Spec")]
    public List<TextureSpec> textureSpec;
}
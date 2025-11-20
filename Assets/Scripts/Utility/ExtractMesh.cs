using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR    // Unity Editor 내에서만 컴파일
using UnityEditor;
#endif
using UnityEngine;


public class ExtractMesh : MonoBehaviour
{
    Mesh mesh;

#if UNITY_EDITOR    // Unity Editor 내에서만 컴파일
    [ContextMenu("ExtractMesh")]    // Inspector 내에서 바로 호출
    void ExtractMeshFrom3DModel()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        string path = "Assets/Resources/Meshes/ExtractedMesh.asset";
        AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
    }
#endif
}
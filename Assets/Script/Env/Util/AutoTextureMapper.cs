using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class UltimateTextureMapper
{
    // 유니티 에디터 상단 메뉴에 최종 실행 항목을 추가합니다.
    [MenuItem("Tools/Run Final Texture Mapper")]
    private static void RunFinalMapping()
    {
        // =========================================================
        // TODO: 여기에 머티리얼 및 텍스처 폴더 경로를 입력하세요.
        // Assets 폴더를 기준으로 상대 경로를 입력해야 합니다.
        // 예시: "Assets/Materials", "Assets/Textures"
        // =========================================================
        string materialsFolderPath = "Assets/Hawaii Beach House (PBR, HDRP)/Materials";
        string texturesFolderPath = "Assets/Hawaii Beach House (PBR, HDRP)/Textures";
        
        // =========================================================

        // 1. 유효성 검사: 폴더 경로가 올바른지 확인합니다.
        if (!AssetDatabase.IsValidFolder(materialsFolderPath) || !AssetDatabase.IsValidFolder(texturesFolderPath))
        {
            Debug.LogError("오류: 지정된 머티리얼 폴더 또는 텍스처 폴더가 유효하지 않습니다. 경로를 확인하세요.");
            return;
        }

        // 2. 텍스처 이름 규칙을 셰이더 속성명과 매핑하는 딕셔너리
        Dictionary<string, string> texturePropertyMap = new Dictionary<string, string>
        {
            {"_albedo", "_BaseColorMap"},
            {"_basecolor", "_BaseColorMap"},
            {" base color", "_BaseColorMap"},
            {"_basemap", "_BaseColorMap"},
            {"_normal", "_BumpMap"},
            {"_roghness", "_MetallicGlossMap"},
            {"_roughness", "_MetallicGlossMap"},
            {"_metallic", "_MetallicGlossMap"},
            {"_height", "_ParallaxMap"},
            {"_parallax", "_ParallaxMap"},
        };

        // 3. 텍스처 파일들을 순회하며 머티리얼 기본 이름으로 그룹화합니다.
        Dictionary<string, List<Texture2D>> textureGroups = new Dictionary<string, List<Texture2D>>();
        string[] allTextureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new[] { texturesFolderPath });

        foreach (string guid in allTextureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string textureFileName = Path.GetFileNameWithoutExtension(path).ToLower();
            
            string baseName = textureFileName;
            foreach(var entry in texturePropertyMap)
            {
                if(baseName.Contains(entry.Key))
                {
                    baseName = baseName.Replace(entry.Key, "").Trim();
                    break;
                }
            }

            if (!string.IsNullOrEmpty(baseName))
            {
                if (!textureGroups.ContainsKey(baseName))
                {
                    textureGroups[baseName] = new List<Texture2D>();
                }
                textureGroups[baseName].Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
        }
        
        // 4. 머티리얼들을 순회하며 텍스처 그룹을 찾아 할당합니다.
        string[] allMaterialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { materialsFolderPath });
        
        foreach (string guid in allMaterialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            string materialName = Path.GetFileNameWithoutExtension(path).ToLower();
            
            string baseMaterialName = materialName;
            foreach(var entry in texturePropertyMap)
            {
                if(baseMaterialName.Contains(entry.Key))
                {
                    baseMaterialName = baseMaterialName.Replace(entry.Key, "").Trim();
                    break;
                }
            }
            
            if (textureGroups.ContainsKey(baseMaterialName))
            {
                Debug.Log($"<color=green>'{material.name}' 머티리얼에 텍스처를 할당합니다.</color>");
                
                foreach (Texture2D texture in textureGroups[baseMaterialName])
                {
                    string textureFileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(texture)).ToLower();
                    
                    foreach(var entry in texturePropertyMap)
                    {
                        if(textureFileName.Contains(entry.Key))
                        {
                            string propertyName = entry.Value;
                            if (material.HasProperty(propertyName))
                            {
                                if (propertyName == "_BumpMap")
                                {
                                    TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
                                    if (textureImporter != null && textureImporter.textureType != TextureImporterType.NormalMap)
                                    {
                                        textureImporter.textureType = TextureImporterType.NormalMap;
                                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture));
                                    }
                                    material.EnableKeyword("_NORMALMAP");
                                }
                                else if (propertyName == "_ParallaxMap")
                                {
                                    material.EnableKeyword("_PARALLAXMAP");
                                }

                                material.SetTexture(propertyName, texture);
                                Debug.Log($"  - 할당된 텍스처: {texture.name} -> 셰이더 속성: {propertyName}");
                            }
                            // 베이스 맵이 할당되지 않을 경우, 다른 속성명들을 추가로 시도합니다.
                            else if (entry.Key.Contains("albedo") || entry.Key.Contains("basecolor") || entry.Key.Contains("basemap"))
                            {
                                if(material.HasProperty("_BaseMap"))
                                {
                                    material.SetTexture("_BaseMap", texture);
                                    Debug.Log($"  - 할당된 텍스처: {texture.name} -> 셰이더 속성: _BaseMap");
                                }
                                else if (material.HasProperty("_MainTex"))
                                {
                                    material.SetTexture("_MainTex", texture);
                                    Debug.Log($"  - 할당된 텍스처: {texture.name} -> 셰이더 속성: _MainTex");
                                }
                            }
                            break;
                        }
                    }
                }
                EditorUtility.SetDirty(material);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=cyan><b>텍스처 자동 할당 작업이 완료되었습니다.</b></color>");
    }
}
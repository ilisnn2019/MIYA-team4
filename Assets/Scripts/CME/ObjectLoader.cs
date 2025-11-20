using UnityEngine;
using System.Collections.Generic;

public class ObjectLoader : MonoBehaviour
{
    [System.Serializable]
    public class ObjectInfoList
    {
        public List<EntityInfo> objects;
    }

    void Start()
    {
        LoadObjects();
    }

    void LoadObjects()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("TestData");
        if (jsonText == null)
        {
            Debug.LogError("TestData.json not found in Resources!");
            return;
        }

        ObjectInfoList objectList = JsonUtility.FromJson<ObjectInfoList>("{\"objects\":" + jsonText.text + "}");

        foreach (var objInfo in objectList.objects)
        {
            SpawnObject(objInfo);
        }
    }

    void SpawnObject(EntityInfo info)
    {
        GameObject prefab = GetPrefabForType(info.type);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for type: {info.type}");
            return;
        }

        GameObject obj = Instantiate(prefab, info.position, Quaternion.Euler(0, info.rotation, 0));
        obj.transform.localScale *=  info.factor;

        // 색상 적용
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && ColorUtility.TryParseHtmlString(info.color, out Color parsedColor))
        {
            renderer.material.color = parsedColor;
        }

        //{
        //    textureName = info.texture;
        //    weight = info.weight;
        //}

        // Registry 등록
        var reference = obj.AddComponent<EntityInfoAgent>();
        reference.LoadInfo(info);


        var registry = InterfaceContainer.Resolve<IRegistry>("registry") as GameObjectRegistry;
        if (registry != null)
        {
            string finalId = registry.Register(info.id, reference);
            Debug.Log($"Registered GameObject with ID: {finalId}");
        }
        else
        {
            Debug.LogWarning("GameObjectRegistry not found!");
        }


    }


    GameObject GetPrefabForType(string type)
    {
        return Resources.Load<GameObject>($"Prefabs/{type}");
    }
}

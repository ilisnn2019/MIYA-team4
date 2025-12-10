using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityInfoAgent : MonoBehaviour
{
    [SerializeField] private EntityInfo objectInfo = new();
    private Dictionary<string, (string prev, string cur)> changeLog = new();

    public EntityInfo Info => objectInfo;
    public Dictionary<string, (string prev, string cur)> ChangeLog => changeLog;

    public void Initialize(string id)
    {
        // 에이전트가 직접 레지스터에 등록
        objectInfo.id = GameObjectRegistry.Instance.Register(id, this);

        objectInfo.position = transform.position;

        var meshRenderer = GetComponent<MeshRenderer>(); // 사이즈 할당
        if (meshRenderer != null)
        {
            Vector3 worldSize = meshRenderer.bounds.size;
            objectInfo.size = worldSize;
        }
    }

    public void OnDestroy()
    {
        // 파괴시 레지트터에서 스스로 삭제
        GameObjectRegistry.Instance.Unregister(objectInfo.id);
    }

    public void LoadInfo(EntityInfo newInfo)
    {
        if (newInfo == null) return;
        objectInfo = newInfo;
    }

    public string SerializeToJson()
    {
        objectInfo.position = transform.position;

        return JsonUtility.ToJson(objectInfo, false);
    }

    public void UpdateProperty(string propertyName, object value)
    {
        string prop = propertyName.ToLower();
        switch (prop)
        {
            case "position":
                if (value is Vector3 pos)
                {
                    var prev = objectInfo.position.ToString();
                    objectInfo.position = pos;
                    changeLog["position"] = (prev, pos.ToString());
                }
                break;
            case "rotation":
                if (value is float rot)
                {
                    var prev = objectInfo.rotation.ToString();
                    objectInfo.rotation = rot;
                    changeLog["rotation"] = (prev, rot.ToString());
                }
                break;
            case "factor":
                if (value is float f)
                {
                    var prev = objectInfo.factor.ToString();
                    objectInfo.factor = f;
                    changeLog["factor"] = (prev, f.ToString());

                    var meshRenderer = GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        Vector3 worldSize = meshRenderer.bounds.size;
                        objectInfo.size = worldSize;
                    }
                }
                break;
            case "color":
                if (value is string col)
                {
                    var prev = objectInfo.color;
                    objectInfo.color = col;
                    changeLog["color"] = (prev, col);
                }
                break;
            case "texture":
                if (value is string tex)
                {
                    var prev = objectInfo.texture;
                    objectInfo.texture = tex;
                    changeLog["texture"] = (prev, tex);
                }
                break;
            case "weight":
                if (value is float w)
                {
                    var prev = objectInfo.weight.ToString();
                    objectInfo.weight = w;
                    changeLog["weight"] = (prev, w.ToString());
                }
                break;
        }
    }

    public string GetObjectType()
    {
        return Info.type;
    }

    public void ClearChanges()
    {
        changeLog.Clear();
    }

    /// <summary>
    /// 현재 게임 오브젝트의 상태(Transform 등)를 EntityInfo에 반영
    /// </summary>
    public void SyncFromGameObject()
    {
        UpdateProperty("position", transform.position);

        UpdateProperty("rotation", transform.eulerAngles.y);

        UpdateProperty("factor", transform.localScale.x);

        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        UpdateProperty("color", ColorUtility.ToHtmlStringRGBA(material.color));

        UpdateProperty("texture", material.GetTexture("_Texture2D").name);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityInfoAgent))]
public class EntityInfoAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EntityInfoAgent agent = (EntityInfoAgent)target;

        DrawDefaultInspector();
        if (GUILayout.Button("현재 정보 반영"))
        {
            agent.SyncFromGameObject();
        }
    }
}
#endif

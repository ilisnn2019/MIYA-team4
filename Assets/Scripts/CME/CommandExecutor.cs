using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Linq;

public class CommandExecutor : MonoBehaviour
{
    private const string HEAD = "CMDEXE";

    FunctionDispatcher dispatcher;
    GameObjectRegistry registry;
    CmdStorage storage;
    //ObjectLoader ObjectLoader = new();
    GameObject last_Select_Object = null; // for highlighting object

    private SpawnNearFacing moveAssist;

    const string PROB_TEXTURE = "_Texture2D";
    const string PROB_COLOR = "_Color";
    const string PROB_CUTOFF = "_Cutoff_Height";

    private Light sun = new();

    public float maxDistance = 5f;
    public Action OnCommandExcuteCallback;

    private readonly Dictionary<string, GameObject> type_entity_pair = new();
    private readonly Dictionary<string, Texture> name_texture_pair = new();

    private void Awake()
    {
        dispatcher = new(this);
        registry = new();
        storage = new();
        //ObjectLoader = GetComponent<ObjectLoader>();
        moveAssist = GetComponent<SpawnNearFacing>();
        sun = RenderSettings.sun;
    }

    private void Start()
    {
        
    }

    public void SetConfiguration(EntityPack epack, TexturePack tpack)
    {
        foreach(var espec in epack.entities)
        {
            type_entity_pair[espec.entityType] = espec.prefab;
        }

        foreach(var tspec in tpack.textureSpec)
        {
            name_texture_pair[tspec.identifier] = tspec.texture;
        }
    }

    public void Dispatcher(string json)
    {
        dispatcher.Dispatch(json, OnCommandExcuteCallback);
    }

    // ===============================
    //  available function
    // ===============================
    public void create_object(string object_type, string id)
    {
        string type = StringUtils.ToLowerSafe(object_type);

        Transform cam = Camera.main.transform;
        Ray ray = new Ray(cam.position, cam.forward);
        Vector3 spawnPosition = new();

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {

            spawnPosition = hit.point;
        }
        else
        {
            spawnPosition = cam.position + cam.forward * maxDistance;
        }


        if (!type_entity_pair.TryGetValue(object_type, out GameObject value))
        {
            Debug.LogError("Error : not found object type");
            return;
        }

        GameObject instanceObject = Instantiate(value);

        instanceObject.transform.position = moveAssist.FindSafeSpawnPosition(instanceObject, spawnPosition);

        var mr = instanceObject.GetComponent<MeshRenderer>();
        StartCoroutine(AnimateCutoff(mr.materials[0], 1.2f, -1.2f));

        EntityInfoAgent agent = instanceObject.GetComponent<EntityInfoAgent>();
        agent.Initialize(id);

        instanceObject.name = object_type + "_" + id;

        storage.RegisterCreate(agent);

        if(EventsManager.instance != null)
        {
            EventsManager.instance.conditionEvents.SpawnedObject(agent);
            Debug.Log("생성 이벤트 발생 : " + agent);
        }
            

        SetDebug(FormatMessage($"Creating, Object ID : {id} at {spawnPosition}"));
    }

    public void rotate_object(string target, float angle)
    {
        ApplyToObject(target, reference =>
        {
            float prev = reference.transform.rotation.y;
            reference.transform.Rotate(0, angle, 0);
            reference.UpdateProperty("rotation", angle);

            storage.RegisterUpdate(reference);
            SetDebug(FormatMessage("Rotating", target, angle));
        });
    }

    public void move_object(string target, float x, float y, float z)
    {
        ApplyToObject(target, reference =>
        {
            Vector3 pos = new Vector3(x,y, z);
            Vector3 newPos = moveAssist.FindSafeSpawnPosition(reference.gameObject, pos);
            //Vector3 newPos = pos;

            reference.transform.position = newPos;
            reference.UpdateProperty("position", newPos);

            storage.RegisterUpdate(reference);
            SetDebug(FormatMessage("Moving", target, x, y, z));
        });
    }

    public void scale_object(string target, float factor)
    {
        ApplyToObject(target, reference =>
        {
            reference.transform.localScale *= factor;
            reference.UpdateProperty("factor", factor);

            storage.RegisterUpdate(reference);
            SetDebug(FormatMessage("Scaling", target, factor));
        });
    }

    public void set_color(string target, string color, float brightness = 0)
    {
        ApplyToObject(target, reference =>
        {
            if (UnityEngine.ColorUtility.TryParseHtmlString(color, out Color parsedColor))
            {
                float Adjust(float channel) =>
                    Mathf.Clamp01(channel + brightness);

                Color result = new Color(
                    Adjust(parsedColor.r),
                    Adjust(parsedColor.g),
                    Adjust(parsedColor.b)
                );

                var renderer = GetOrAdd<MeshRenderer>(reference.gameObject);
                Color startcolor = renderer.materials[0].GetColor(PROB_COLOR);

                StartCoroutine
                (
                    ColorUtil.LerpColor
                    (
                        startcolor, result,
                        (c)=>
                        {
                            renderer.materials[0].SetColor(PROB_COLOR,c);
                        }
                    )
                );

                string hex = $"#{UnityEngine.ColorUtility.ToHtmlStringRGB(result)}";
                reference.UpdateProperty("color", hex);

                storage.RegisterUpdate(reference);
                SetDebug(FormatMessage("Setting color", target, result.r, result.g, result.b));
            }
            else
            {
                Debug.LogWarning($"Invalid color format: {color}");
            }
        });
    }


    public void set_weight(string target, float weight)
    {
        ApplyToObject(target, reference =>
        {
            var rb = GetOrAdd<Rigidbody>(reference.gameObject);
            var grabbale = GetOrAdd<XRGrabInteractable>(reference.gameObject);
            rb.mass = weight;
            reference.UpdateProperty("weight", weight);

            storage.RegisterUpdate(reference);
            SetDebug(FormatMessage("Setting weight", target, weight));
        });
    }

    public void set_texture(string target, string texture)
    {
        ApplyToObject(target, reference =>
        {
            string textureName = StringUtils.ToLowerSafe(texture);

            if (!name_texture_pair.TryGetValue(textureName, out Texture value))
            {
                Debug.LogError("Error : not found texture");
                return;
            }

            if (value != null)
            {
                var renderer = GetOrAdd<MeshRenderer>(reference.gameObject);
                renderer.materials[0].SetTexture(PROB_TEXTURE, value);
                reference.UpdateProperty("texture", texture);

                storage.RegisterUpdate(reference);
                SetDebug($"Setting texture of {target} to {texture}");
            }
            else
            {
                Debug.LogWarning($"Texture '{textureName}' not found");
                SetDebug($"Failed to set texture: Texture '{textureName}' not found.");
            }
        });

    }

    public void select_object(string target)
    {
        GameObject obj = registry.Resolve(target).gameObject;
        if (last_Select_Object != null)
            last_Select_Object.GetComponent<Outline>().enabled = false;
        var outline = GetOrAdd<Outline>(obj);
        outline.enabled = true;
        last_Select_Object = obj;

        //obj.transform.localScale *= 1.2f;
        SetDebug($"Selected object: {target}");
    }

    public void handle_unknown_command(string reason)
    {
        SetDebug($"Cannot find the proper function: {reason}");
    }

    public void remove_object(string target)
    {
        var obj = registry.Unregister(target);

        StartCoroutine(AnimateCutoff(obj.GetComponent<MeshRenderer>().material, -1.5f, 1.5f, 
            callback: () => { Destroy(obj); } 
        ));

    }

    #region hand-free vr function
    [Header("Use only for hand-free VR")]
    public Transform instantiatePosition;
    List<EntityInfoAgent> selected_agents = new();

    public void create(string object_type)
    {
        string type = StringUtils.ToLowerSafe(object_type);

        GameObject go = Instantiate(type_entity_pair[type]);

        // 위치/기본 설정
        go.transform.position = instantiatePosition.position;      // 필요 시 파라미터로 변경 가능
        go.transform.localScale = Vector3.one;

        // 생성된 객체가 EntityInfoAgent를 가지면 자동 등록
        var agent = go.GetComponent<EntityInfoAgent>();
        agent.Initialize("");
    }

    public void select_all()
    {
        selected_agents.Clear();
        selected_agents = registry.GetAllAgents().ToList();
    }

    public void select(string object_type)
    {
        string type = StringUtils.ToLowerSafe(object_type);
        selected_agents.Clear();
        foreach (var agent in registry.GetAllAgents())
        {
            if (agent.Info.type.Equals(type)) selected_agents.Add(agent);
        }
    }

    private readonly Dictionary<string, string> ColorNameToHex = new()
    {
        { "white", "#FFFFFF" },
        { "black", "#000000" },
        { "red", "#FF0000" },
        { "green", "#00FF00" },
        { "blue", "#0000FF" },
        { "yellow", "#FFFF00" },
        { "cyan", "#00FFFF" },
        { "magenta", "#FF00FF" },
        { "gray", "#808080" },
    };

    public void select(string object_type, string color)
    {
        string type = StringUtils.ToLowerSafe(object_type);
        string targetHex = GetHexFromName(color);

        if (targetHex == null)
        {
            Debug.LogWarning($"Unknown color name: {color}");
            return;
        }

        selected_agents.Clear();

        foreach (var agent in registry.GetAllAgents())
        {
            if (agent.Info.type.Equals(type) &&
                agent.Info.color.Equals(targetHex, StringComparison.OrdinalIgnoreCase))
            {
                selected_agents.Add(agent);
            }
        }
    }

    private string GetHexFromName(string name)
    {
        name = StringUtils.ToLowerSafe(name);
        return ColorNameToHex.TryGetValue(name, out var hex) ? hex : null;
    }

    public void move_to_sphere(string object_type)
    {
        string type = StringUtils.ToLowerSafe(object_type);
        Vector3 spherePosition = Vector3.zero;
        foreach (var agent in registry.GetAllAgents())
        {
            if (agent.Info.type.Equals("sphere"))
            {
                spherePosition = agent.transform.position;
                break;
            }
        }
        foreach(var agent in selected_agents)
        {
            agent.transform.position = spherePosition;
        }
    }
    public void arrange(string mode)
    {
        mode = StringUtils.ToLowerSafe(mode);

        switch (mode)
        {
            case "row":
                ArrangeRow();
                break;

            case "matrix":
                ArrangeMatrix();
                break;

            case "circle":
                ArrangeCircle();
                break;

            default:
                Debug.LogWarning($"Unknown arrange mode: {mode}");
                break;
        }
    }
    private void ArrangeRow()
    {
        float spacing = 2f; // 간격

        for (int i = 0; i < selected_agents.Count; i++)
        {
            var agent = selected_agents[i];
            agent.transform.position = new Vector3(i * spacing, 0, 0) + instantiatePosition.position;
        }
    }
    private void ArrangeMatrix()
    {
        int count = selected_agents.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)columns);

        float spacing = 2f;

        for (int i = 0; i < count; i++)
        {
            var agent = selected_agents[i];

            int row = i / columns;
            int col = i % columns;

            agent.transform.position = new Vector3(col * spacing, 0, row * spacing) + instantiatePosition.position;
        }
    }
    private void ArrangeCircle()
    {
        int count = selected_agents.Count;
        float radius = 5f;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            selected_agents[i].transform.position = new Vector3(x, 0, z) + instantiatePosition.position;
        }
    }
    #endregion
    // ----------- Util Func ------------

    private T GetOrAdd<T>(GameObject obj) where T : Component
    {
        return obj.TryGetComponent<T>(out var comp) ? comp : obj.AddComponent<T>();
    }

    private void ApplyToObject(string id, Action<EntityInfoAgent> action)
    {
		EntityInfoAgent fr = registry.Resolve(id);
        if (fr != null)
        {
            action(fr);
            // set curious object

        }
        else
            Debug.LogWarning($"Object '{id}' not found.");
    }

    private string FormatMessage(string action, string name, float x, float y = 0, float z = 0, string extra = "")
    {
        string coords = $"({x}, {y}, {z})";
        return $"{action} {name} at {coords}" + (string.IsNullOrEmpty(extra) ? "" : $"\n{extra}");
    }

    private string FormatMessage(string action, string name, float value)
    {
        return $"{action} of {name} to {value}";
    }

    private string FormatMessage(string v)
    {
        return v;
    }

    IEnumerator AnimateCutoff(Material material, float start, float end, float duration = 1.5f, Action callback = null)
    {
        float elapsed = 0f;
        material.SetFloat(PROB_CUTOFF, start);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float value = Mathf.Lerp(start, end, t);
            material.SetFloat(PROB_CUTOFF, value);
            yield return null;
        }

        // ������ �� ����
        material.SetFloat(PROB_CUTOFF, end);
        callback?.Invoke();
    }

    private void SetDebug(string message)
    {
#if UNITY_EDITOR
        TimerLogger.EndTimer("cmd dispatch");
        GText.SaveLine("Output", "����� �׼�: " + message);
        GText.PrintLines();
        LLog.Log(LogType.Log, HEAD, message);
#endif
    }
}

public static class StringUtils
{
    public static string ToLowerSafe(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.ToLower();
    }
}

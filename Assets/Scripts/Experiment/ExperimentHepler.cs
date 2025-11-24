using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

public class ExperimentHepler : MonoBehaviour
{
    Dictionary<string, GameObject> instanceEntity = new();
    Dictionary<string, Texture> instanceTexture = new();
    public EntityPack EntityPack;
    public TexturePack TexturePack;

    private void Start()
    {
        foreach (var entity in EntityPack.entities)
        {
            instanceEntity[entity.entityType] = entity.prefab;
        }
        foreach (var texture in TexturePack.textureSpec)
        {
            instanceTexture[texture.identifier] = texture.texture;
        }
    }

    public void Create(string data)
    {
        string[] tokens = data.Split(':');
        EntityInfoAgent currentAgent = null;
        string currentProperty = "";

        for (int i = 0; i < tokens.Length; i++)
        {
            
            string token = tokens[i].ToLower();
            if (token == "type")
            {
                if (i + 1 < tokens.Length)
                {
                    string typeName = tokens[++i].ToLower();
                    currentAgent = Instantiate(instanceEntity[typeName]).GetComponent<EntityInfoAgent>();
                    currentAgent.Initialize("");
                    currentProperty = null;
                }
                continue;
            }
            
            if (currentProperty == null)
            {
                currentProperty = token;
                continue;
            }
            if (currentAgent != null)
            {
                object parsedValue = ParseValue(currentProperty, token);
                UpdateProperty(currentAgent, currentProperty, parsedValue);
            }

            currentProperty = null;
        }
    }
    public void RemoveAllEntity()
    {
        foreach (var entity in FindObjectsByType<EntityInfoAgent>(FindObjectsSortMode.None))
        {
            Destroy(entity.gameObject);
        }
    }
    private void UpdateProperty(EntityInfoAgent currentAgent, string currentProperty, object value)
    {
        switch (currentProperty)
        {
            case "position":
                if (value is Vector3 pos)
                {
                    currentAgent.transform.position = pos;
                }
                break;

            case "rotation":
                if (value is float rot)
                {
                    currentAgent.transform.eulerAngles = new Vector3(0,rot,0);
                }
                break;

            case "factor":
                if (value is float factor)
                {
                    currentAgent.transform.localScale *= factor;
                }
                break;

            case "color":
                if (value is string col)
                {
                    if (UnityEngine.ColorUtility.TryParseHtmlString(col, out UnityEngine.Color parsedColor))
                    {
                        var renderer = currentAgent.GetComponent<Renderer>();
                        renderer.materials[0].SetColor("_Color", parsedColor);
                    }
                }
                break;

            case "texture":
                if (value is string tex)
                {
                    var texture = instanceTexture[tex];
                    var renderer = currentAgent.GetComponent<Renderer>();
                    renderer.materials[0].SetTexture("_Texture2D", texture);
                }
                break;

            case "weight":
                if (value is float w)
                {
                    var rb = currentAgent.GetComponent<Rigidbody>();
                    rb.mass = w;
                }
                break;
        }

        currentAgent.UpdateProperty(currentProperty, value);
    }

    private object ParseValue(string propertyName, string value)
    {
        switch (propertyName)
        {
            case "position":
                return ToVector3(value);

            case "rotation":
                return float.Parse(value);

            case "factor":
                return float.Parse(value);

            case "weight":
                return float.Parse(value);

            case "color":
                return value;
            case "texture":
                return value;

            default:
                return value;
        }
    }
    private Vector3 ToVector3(string s)
    {
        string[] parts = s.Split(',');
        if (parts.Length != 3) return Vector3.zero;

        return new Vector3(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2])
        );
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FunctionCollection
{
    private readonly List<FunctionDefinition> functions;
    private readonly List<string> names;

    private const string filepath = "FunctionData";

    public FunctionCollection()
    {
        functions = LoadFromResources(filepath);
        names = functions.Select(f => f.name).ToList();
    }

    public FunctionDefinition GetDefinitionByName(string name)
    {
        return functions.FirstOrDefault(f => f.name == name);
    }

    public string GetDefinitionJsonByName(string name)
    {
        var definition = GetDefinitionByName(name);
        return definition != null ? JsonConvert.SerializeObject(definition) : null;
    }

    private List<FunctionDefinition> LoadFromResources(string fileNameWithoutExtension)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileNameWithoutExtension);
        if (textAsset == null)
        {
            Debug.LogError($"FunctionCollection: Cannot find file in Resources/{fileNameWithoutExtension}.json");
            return new List<FunctionDefinition>();
        }

        try
        {
            return JsonConvert.DeserializeObject<List<FunctionDefinition>>(textAsset.text);
        }
        catch (Exception e)
        {
            Debug.LogError($"FunctionCollection: JSON parse error - {e.Message}");
            return new List<FunctionDefinition>();
        }
    }
}

using System.Collections.Generic;
using System;
using UnityEngine;
using Mono.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

interface IRegistry
{
    EntityInfoAgent[] GetAllAgents();
    string Register(string id, EntityInfoAgent reference);
    EntityInfoAgent Resolve(string id);
    string ChangeId(string oldId);
}

/// <summary>
/// 만들어진, 수정 가능한 물체를 저장한 컨테이너 클래스
/// 아이디로 물체 탐색 가능
/// </summary>
public class GameObjectRegistry : IRegistry
{
    public static GameObjectRegistry Instance;

    public GameObjectRegistry()
    {
        Instance = this;
        InterfaceContainer.Register<IRegistry>("registry", this);
    }

    private readonly Dictionary<string, EntityInfoAgent> map = new();

    public EntityInfoAgent[] GetAllAgents() => map.Values.ToArray();

    public string Register(string id, EntityInfoAgent reference)
    {
        if (string.IsNullOrEmpty(id))
            id = RandomStringUtil.Generate10();

        id = EnsureFixedLengthId(id);

        while (!map.TryAdd(id, reference))
        {
            id = RandomStringUtil.Generate10();
        }

        Debug.Log($"register in registry : {id}");
        return id;
    }

    public EntityInfoAgent Resolve(string id)
    {
        map.TryGetValue(id, out var obj);
        return obj;
    }

    public GameObject Unregister(string id)
    {
        if (!map.TryGetValue(id, out EntityInfoAgent value))
        {
            return null;
        }

        map.Remove(id);
        return value.gameObject;
    }

    public string ChangeId(string oldId)
    {
        if (!map.TryGetValue(oldId, out var reference))
            throw new ArgumentException($"Old ID {oldId} not found in registry.");

        string newId = RandomStringUtil.Generate10();

        while (!map.TryAdd(newId, reference))
        {
            newId = RandomStringUtil.Generate10();
        }

        map.Remove(oldId);

        reference.Info.id = newId;

        LLog.Log(LogType.Log, "REG-CHANGE", $"{oldId} → {newId}");
        return newId;
    }

    private string EnsureFixedLengthId(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty.");

        if (id.Length > 10)
            return id.Substring(0, 10);
        else if (id.Length < 10)
            return id.PadRight(10, '0');
        else
            return id;
    }
}

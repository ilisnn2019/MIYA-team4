using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

interface IStorage
{
    void RegisterCreate(EntityInfoAgent fr);
    void RegisterUpdate(EntityInfoAgent fr);
    string GetChanged();
    void Clear();
    string PrevCmd { get; set; }
    string[] GetCreateds();
    string[] GetUpdateds();
}

/// <summary>
/// Snapshot of a furniture at a given time.
/// </summary>
public class FurnitureSnapshot
{
    public string Type { get; set; }
    public string Size { get; set; }
    public string Position { get; set; }
    public string Rotation { get; set; }
    public string Factor { get; set; }
    public string Color { get; set; }
    public string Texture { get; set; }
    public string Weight { get; set; }

    public static FurnitureSnapshot From(EntityInfoAgent fr) => new()
    {
        Type = fr.Info.type,
        Position = fr.Info.position.ToString(),
        Rotation = fr.Info.rotation.ToString(),
        Factor = fr.Info.factor.ToString(),
        Color = fr.Info.color,
        Texture = fr.Info.texture,
        Weight = fr.Info.weight.ToString()
    };
}

/// <summary>
/// A class that records the values changing of data after executing command.
/// </summary>
public class CmdStorage : IStorage
{
    public string PrevCmd { get; set; }

    // alias type definitions for readability
    private readonly Dictionary<string, FurnitureSnapshot> created = new();
    private readonly Dictionary<string, Dictionary<string, (string prev, string cur)>> updated = new();

    public CmdStorage()
    {
        InterfaceContainer.Register<IStorage>("storage", this);
    }

    public string[] GetCreateds() => created.Keys.ToArray();
    public string[] GetUpdateds() => updated.Keys.ToArray();

    public void RegisterCreate(EntityInfoAgent fr)
    {
        if (fr == null || string.IsNullOrEmpty(fr.Info.id)) return;
        created[fr.Info.id] = FurnitureSnapshot.From(fr);
    }

    public void RegisterUpdate(EntityInfoAgent fr)
    {
        if (fr == null || string.IsNullOrEmpty(fr.Info.id)) return;
        if (fr.ChangeLog.Count == 0) return;

        updated[fr.Info.id] = new Dictionary<string, (string prev, string cur)>(fr.ChangeLog);
    }

    public string GetChanged()
    {
        var result = new
        {
            CREATE = created,
            UPDATE = updated.ToDictionary(
                kv => kv.Key,
                kv => new {
                    previous = kv.Value.ToDictionary(p => p.Key, p => p.Value.prev),
                    current = kv.Value.ToDictionary(p => p.Key, p => p.Value.cur)
                })
        };

        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }

    public void Clear()
    {
        created.Clear();
        updated.Clear();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public static class InterfaceContainer
{
    private static readonly Dictionary<string, object> _services = new();

    public static void Register<T>(string key, T instance) where T : class
    {
        var type = typeof(T);
        if (!_services.ContainsKey(key))
        {
            _services.Add(key, instance);
        }
        else
        {
            Debug.LogWarning($"Service of type {type.Name} is already registered.");
        }
    }

    public static T Resolve<T>(string key) where T : class
    {
        var type = typeof(T);
        if (_services.TryGetValue(key, out var service))
        {
            return service as T;
        }
        Debug.LogWarning($"Service of type {type.Name} not found.");
        return null;
    }

    public static bool Unregister(string key)
    {
        return _services.Remove(key);
    }
}

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Newtonsoft.Json;
public class DispatchFunction
{
    public string name;
    public JObject arguments;
}

public class FunctionDispatcher
{
    private readonly CommandExecutor _executor;
    private readonly Dictionary<string, MethodInfo> _methodMap;

    public FunctionDispatcher(CommandExecutor executor)
    {
        _executor = executor;
        _methodMap = new Dictionary<string, MethodInfo>();

        var methods = executor.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            _methodMap[method.Name.ToLower()] = method;
        }
    }

    public void Dispatch(string json, Action callback)
    {
        try
        {
            var dispatchFunctions = JsonConvert.DeserializeObject<List<DispatchFunction>>(json);
            if (dispatchFunctions == null)
            {
                Debug.LogError("No valid function calls in JSON.");
                return;
            }

            foreach (var call in dispatchFunctions)
            {
                string funcName = call.name.Replace("functions.", "").ToLower();

                if (_methodMap.TryGetValue(funcName, out MethodInfo method))
                {
                    var parameters = method.GetParameters();
                    var args = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];

                        if (!call.arguments.TryGetValue(param.Name, StringComparison.OrdinalIgnoreCase, out JToken token))
                        {
                            token = FindTokenRecursive(call.arguments, param.Name);
                        }

                        if (token != null)
                        {
                            args[i] = token.ToObject(param.ParameterType);
                        }
                        else
                        {
                            args[i] = param.HasDefaultValue ? param.DefaultValue : GetDefault(param.ParameterType);
                        }
                    }

                    method.Invoke(_executor, args);
                }
                else
                {
                    Debug.LogWarning($"No matching method for function name: {funcName}");
                }
            }

            callback?.Invoke();
        }
        catch (TargetInvocationException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Dispatch error: " + e.Message);
        }


    }

    private JToken FindTokenRecursive(JObject obj, string paramName)
    {
        foreach (var property in obj.Properties())
        {
            if (string.Equals(property.Name, paramName, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }

            if (property.Value.Type == JTokenType.Object)
            {
                var found = FindTokenRecursive((JObject)property.Value, paramName);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}


using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class DebugMonitor : MonoBehaviour
{
    public List<EntityInfoAgent> agents = new();

    [ContextMenu("view all entities in present scene")]
    public void ViewAllEntitiesInScene()
    {
        var reg = InterfaceContainer.Resolve<IRegistry>("registry");
        agents.Clear();
        agents = reg.GetAllAgents().ToList();
        Debug.Log(reg.GetAllAgents().Length);    
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using RealismModSync.StanceReplication.Components;

namespace RealismModSync.StanceReplication;

public static class Core
{
    public static ConcurrentDictionary<int, RSR_Observed_Component> ObservedComponents;
    
    public static void Initialize()
    {
        ObservedComponents = new ConcurrentDictionary<int, RSR_Observed_Component>();
    }
}
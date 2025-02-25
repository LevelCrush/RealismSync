using System.Collections.Concurrent;
using System.Collections.Generic;
using RealismMod;
using RealismModSync.StanceReplication.Components;

namespace RealismModSync.HazardZones;

public static class Core
{

    // key = [zoneType]||[zoneName]
    // value = Result of original method ShouldZoneSpawn. For Server this will be populated
    // for everyone else this will be empty
    // a packet will take care of populating this information for clients
    public static ConcurrentDictionary<string, bool> ZoneResults;
    public static ConcurrentDictionary<string, HazardGroup> HazardGroups;
    
    public static void Initialize()
    {
        ZoneResults = new ConcurrentDictionary<string, bool>();
        HazardGroups = new ConcurrentDictionary<string, HazardGroup>();
        
    }
    
    
    public static string GenerateZoneKey(HazardGroup hazardLocation, EZoneType zoneType)
    {
        // generate key for this hazard location group
        var zoneNamesList = new List<string>();
        for (var i = 0; i < hazardLocation.Zones.Count; i++)
        {
            zoneNamesList.Add($"{i}.{hazardLocation.Zones[i].Name}");
        }
        
        var zoneNames = string.Join("-", zoneNamesList);
        var zoneKey = $"({(int)zoneType}||{zoneNames})";
        return zoneKey;
    }
}
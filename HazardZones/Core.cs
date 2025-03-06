using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealismMod;
using RealismModSync.StanceReplication.Components;
using UnityEngine;

namespace RealismModSync.HazardZones;

public static class Core
{

    // key = [zoneType]||[zoneName]
    // value = Result of original method ShouldZoneSpawn. For Server this will be populated
    // for everyone else this will be empty
    // a packet will take care of populating this information for clients
    public static ConcurrentDictionary<string, bool> ZoneResults;
    public static ConcurrentDictionary<string, HazardGroup> HazardGroups;
    public static ConcurrentDictionary<string, bool> ZoneWillSpawn;
    
    public static void Initialize()
    {
        ZoneResults = new ConcurrentDictionary<string, bool>();
        HazardGroups = new ConcurrentDictionary<string, HazardGroup>();
        ZoneWillSpawn = new ConcurrentDictionary<string, bool>();
        
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
        var zoneKey = $"{zoneType}||{zoneNames}";
        return zoneKey;
    }

    public static void LoadAsset(string assetName, Vector3 position, Vector3 rotation)
    {
        GameObject assetPrefab = ZoneSpawner.GetAndLoadAsset(assetName);
        if (assetPrefab == null) 
        {
            Plugin.REAL_Logger.LogError("RealismSync Mod: Error Loading Asset From Bundle For Asset: " + assetName);
        }
        GameObject spawnedAsset = UnityEngine.Object.Instantiate(assetPrefab, position, Quaternion.Euler(rotation));
    }
    
    public static async Task LoadLooseLoot(Vector3 position, Vector3 rotation, string templateID, string mongoID)
    {
        Quaternion quat = Quaternion.Euler(rotation);
        await RealismModSync.Utils.LoadLoot(position, quat,templateID, mongoID);
    }
}
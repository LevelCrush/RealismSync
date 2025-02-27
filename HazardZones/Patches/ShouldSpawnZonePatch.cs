using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RealismModSync.HazardZones.Patches;

public class ShouldSpawnZonePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ZoneSpawner).GetMethod("ShouldSpawnZone", BindingFlags.Static | BindingFlags.NonPublic);
    }
    
    [PatchPrefix]
    public static bool Prefix(ref bool __runOriginal, HazardGroup hazardLocation, EZoneType zoneType, ref bool __result)
    {
        
        
        // if there are other patches that have already said don't let the original run. 
        // respect it and cancel out
        if (!__runOriginal)
        {
            Plugin.REAL_Logger.LogWarning($"Honoring the previous prefix original method skip. No modification will take place");
            return false;
        }
    
        // generate key for this hazard location group
        var zoneKey = Core.GenerateZoneKey(hazardLocation, zoneType);
        
        if (FikaBackendUtils.IsServer)
        {
            // always run original method if server
            Plugin.REAL_Logger.LogInfo($"Running zone checks for {zoneKey}");
            return true;
        }
        
        // only let original method run **if we have a result already stored and the result stored is true**
        Core.ZoneResults.TryGetValue(zoneKey, out var result);
        Plugin.REAL_Logger.LogInfo($"{result} for {zoneKey}");

        // short circuit and force this result
        __result = result;
    
        // if we had a cache result, run the original method. overwriting what we put into __result. Otherwise skip and return what we put into __result
        return result;
    }

    [PatchPostfix]
    public static void Postfix(HazardGroup hazardLocation, EZoneType zoneType, ref bool __result)
    {
        var zoneKey = Core.GenerateZoneKey(hazardLocation, zoneType);
        Core.ZoneResults.TryGetValue(zoneKey, out var spawnZone);
        
 
        if (FikaBackendUtils.IsClient && !spawnZone)
        {
            // force a false on our result preventing the hazard zone from spawning
            // when the spawn packet comes in, this method will run again and hopefully
            // with the right data populated will run 
            Plugin.REAL_Logger.LogInfo($"Forcing no spawn for  {zoneKey} as there is no result cached");
            __result = false;
        }
        
        // if we are the host, we are going to store this result and send it out via a packet to all connected clients
        if (FikaBackendUtils.IsServer)
        {
            var shouldSpawnResult = __result;
            if (shouldSpawnResult)
            {
                // send packet with zone key
                // we only send packets when we **should** spawn a zone
                Plugin.REAL_Logger.LogInfo($"Sending {zoneKey} to other players to spawn");
                var packet = new RealismHazardPacket()
                {
                    ZoneKey = zoneKey,
                    ZoneType = zoneType
                };
                
                Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
            }
          
        } 
    }
}
using System.Reflection;
using System.Threading.Tasks;
using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RealismModSync.HazardZones.Patches;

public class HandleZoneLootPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ZoneSpawner).GetMethod(nameof(ZoneSpawner.HandleZoneLoot), BindingFlags.Static | BindingFlags.Public);
    }
    
    /**
     * For now until RealismMod is patched. This function should basically match the equivalent HandleZoneLoot but Fika targetd
     * Original Method found here: https://github.com/space-commits/SPT-Realism-Mod-Client/blob/main/GameWorld/ZoneSpawner.cs -> HandleZoneLoot(HazardGroup zone)
     */
    [PatchPrefix]
    public static bool Patch(HazardGroup zone)
    {
        if (FikaBackendUtils.IsClient)
        {
            Plugin.REAL_Logger.LogInfo("Clients do not generate loot in zones");
            return false;
        }
        
        if (zone.Loot == null)
        {
            Plugin.REAL_Logger.LogInfo("No loot found in zone.");
            return false;
        }
        
        foreach (var loot in zone.Loot)
        {
            if (RealismMod.Utils.SystemRandom.Next(101) > loot.Odds) continue;

            if (loot.RandomizeRotation)
            {
                loot.Rotation.Y = RealismMod.Utils.SystemRandom.Range(0, 360);
            }

            Vector3 position = new Vector3(loot.Position.X, loot.Position.Y, loot.Position.Z);
            Vector3 rotation = new Vector3(loot.Rotation.X, loot.Rotation.Y, loot.Rotation.Z);

            string lootTemplateId = loot?.LootOverride != null && loot.LootOverride.Count > 0 ? ZoneSpawner.GetLootTempalteIdFromOverride(loot.LootOverride) : ZoneSpawner.GetLootTempalteIdFromTier(loot.Type);
            var mongoID = RealismMod.Utils.GenId();

            var packet = new RealismLootPacket()
            {
                MongoID = mongoID,
                TemplateId = lootTemplateId,
                Position = position,
                Rotation = rotation,
            };
            
            Plugin.REAL_Logger.LogInfo($"Broadcasting loot {mongoID} to lobby");
            Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
            
            Core.LoadLooseLoot(position, rotation, lootTemplateId, mongoID);
            
        }
        
        return false;
    }



}
using System.Reflection;
using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RealismModSync.HazardZones.Patches;

public class HandleZoneAssetsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ZoneSpawner).GetMethod(nameof(ZoneSpawner.HandleZoneAssets), BindingFlags.Static | BindingFlags.Public);
    }

    /**
   * For now until RealismMod is patched. This function should basically match the equivalent HandleZoneAssets but Fika targetd
   * Original Method found here: https://github.com/space-commits/SPT-Realism-Mod-Client/blob/main/GameWorld/ZoneSpawner.cs -> HandleZoneAssets(HazardGroup zone)
   */
    [PatchPrefix]
    public static bool Patch(HazardGroup zone)
    {
        if (FikaBackendUtils.IsClient)
        {
            Plugin.REAL_Logger.LogInfo("Clients do not choose where to spawn assets");
            return false;
        }
        
        if (zone.Assets == null)
        {
            return false;
        }
    
        foreach (var asset in zone.Assets) 
        {
            if (RealismMod.Utils.SystemRandom.Next(101) > asset.Odds) continue;

            if (asset.RandomizeRotation) 
            {
                asset.Rotation.Y = RealismMod.Utils.SystemRandom.Range(0, 360);
            }

            Vector3 position = new Vector3(asset.Position.X, asset.Position.Y, asset.Position.Z);
            Vector3 rotation = new Vector3(asset.Rotation.X, asset.Rotation.Y, asset.Rotation.Z);
            
            var packet = new RealismAssetPacket()
            {
                AssetName = asset.AssetName,
                Position = position,
                Rotation = rotation
            };

            Plugin.REAL_Logger.LogInfo($"Broadcasting asset {asset.AssetName} at position {position} and rotation {rotation}");
            Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
            
            Core.LoadAsset(asset.AssetName, position, rotation);
        }

        return false;
    }
}
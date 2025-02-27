using System.Collections.Generic;
using System.Reflection;
using RealismMod;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class ZonedDataGetZonesPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ZoneData).GetMethod(nameof(ZoneData.GetZones), BindingFlags.Static | BindingFlags.Public );
    }

    [PatchPostfix]
    public static void PostFix(EZoneType zoneType, List<HazardGroup> __result)
    {
        if (__result != null)
        {
            foreach (var zone in __result)
            {
                var zoneKey = Core.GenerateZoneKey(zone, zoneType);
                Plugin.REAL_Logger.LogInfo($"Zone: {zoneKey} is cached");
                Core.HazardGroups.AddOrUpdate(zoneKey, zone, (s, group) => zone);
            }
        }
    }
}
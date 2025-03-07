using System.Reflection;
using Fika.Core.Coop.Utils;
using RealismMod;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class DoMapGasEventPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorldController).GetProperty(nameof(GameWorldController.DoMapGasEvent)).GetMethod;
    }

    [PatchPrefix]
    public static bool Patch(ref bool __result)
    {
        if (FikaBackendUtils.IsServer)
        {
            // if Fika server. Let it fall through
            return true;
        }
        
        __result = Core.DoMapGasEvent;
        return false;

    }
}
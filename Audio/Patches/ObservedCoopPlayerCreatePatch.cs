using System.Reflection;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.Players;
using RealismModSync.Audio.Components;
using RealismModSync.StanceReplication.Components;
using SPT.Reflection.Patching;

namespace RealismModSync.Audio.Patches;

public class ObservedCoopPlayerCreatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(CoopHandler).GetMethod("SpawnObservedPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [PatchPostfix]
    public static void Postfix(ObservedCoopPlayer __result)
    {
        if (__result.IsObservedAI && !Config.EnableForBots.Value) return;
        __result.gameObject.AddComponent<RSAObservedComponent>();
    }
}
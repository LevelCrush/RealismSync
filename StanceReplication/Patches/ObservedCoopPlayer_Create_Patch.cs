using SPT.Reflection.Patching;
using Fika.Core.Coop.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fika.Core.Coop.Components;
using RealismModSync.StanceReplication.Components;

namespace RealismModSync.StanceReplication.Patches
{
    public class ObservedCoopPlayer_Create_Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(CoopHandler).GetMethod("SpawnObservedPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        public static void Postfix(ObservedCoopPlayer __result)
        {
            if (__result.IsObservedAI && !Config.EnableForBots.Value) return;
            __result.gameObject.AddComponent<RSR_Observed_Component>();
            
            
        }
    }
}

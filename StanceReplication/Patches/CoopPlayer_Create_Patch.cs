using SPT.Reflection.Patching;
using Fika.Core.Coop.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RealismModSync.StanceReplication.Components;

namespace RealismModSync.StanceReplication.Patches
{
    public class CoopPlayer_Create_Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(CoopPlayer).GetMethod(nameof(CoopPlayer.SetupMainPlayer));
        }

        [PatchPostfix]
        public static void Postfix(CoopPlayer __instance)
        {
            __instance.gameObject.AddComponent<RSR_Component>();
        }
    }
}

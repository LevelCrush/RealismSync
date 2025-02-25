using SPT.Reflection.Patching;
using Fika.Core.Coop.Players;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Fika.Core.Coop.GameMode;
using RealismModSync.StanceReplication.Components;

namespace RealismModSync.StanceReplication.Patches
{
    public class CoopBot_Create_Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(CoopGame).GetMethod("CreateBot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        public static async void Postfix(Task<LocalPlayer>__result)
        {
            if (Config.EnableForBots.Value) 
            {
                var res = await __result;
                ((CoopBot)res).gameObject.AddComponent<RSR_Component>();
            }
        }
    }
}

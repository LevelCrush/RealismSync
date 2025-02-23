using System.Reflection;
using Comfort.Common;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.GameMode;
using Fika.Core.Coop.Players;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using LiteNetLib.Utils;
using RealismMod;
using SPT.Reflection.Patching;

namespace StanceReplication
{
    public class RealismLeftShoulderSwapPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            
            Plugin.EnableShoulderSwap.SettingChanged += (sender, args) =>
            {
                if (Plugin.EnableShoulderSwap.Value == false)
                {
                    // disabled. Let's send a packet out to reset this person back to right hand
                    TransitionShoulder(false);
                }
            };
            
            return typeof(StanceController).GetMethod(nameof(StanceController.ToggleLeftShoulder));
        }

        private static void TransitionShoulder(bool leftShoulder)
        {
            /*
            CoopHandler fikaCoopHandler;
            if (CoopHandler.TryGetCoopHandler(out fikaCoopHandler))
            {
                fikaCoopHandler.MyPlayer.PacketSender.FirearmPackets.Enqueue(new WeaponPacket()
                {
                    HasStanceChange = true,
                    LeftStanceState = leftShoulder
                });
            } */
        }

        [PatchPostfix]
        public static void Postfix()
        {
            if (Plugin.EnableShoulderSwap.Value)
            {
                TransitionShoulder(StanceController.IsLeftShoulder);
            }
        }
    }
}
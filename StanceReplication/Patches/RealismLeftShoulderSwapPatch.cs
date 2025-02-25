using System.Reflection;
using Comfort.Common;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.GameMode;
using Fika.Core.Coop.Players;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using Fika.Core.Networking.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using RealismMod;
using SPT.Reflection.Patching;

namespace RealismModSync.StanceReplication.Patches
{
    public class RealismLeftShoulderSwapPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            
            Config.EnableShoulderSwap.SettingChanged += (sender, args) =>
            {
                if (Config.EnableShoulderSwap.Value == false)
                {
                    // disabled. Let's send a packet out to reset this person back to right hand
                    TransitionShoulder(false);
                }
            };
            
            return typeof(StanceController).GetMethod(nameof(StanceController.ToggleLeftShoulder));
        }

        private static void TransitionShoulder(bool leftShoulder)
        {
            
            CoopHandler fikaCoopHandler;
            if (CoopHandler.TryGetCoopHandler(out fikaCoopHandler))
            {
                var leftStanceChangePacket = new FirearmSubPackets.LeftStanceChangePacket();
                leftStanceChangePacket.LeftStance = leftShoulder;
                
                fikaCoopHandler.MyPlayer.PacketSender.FirearmPackets.Enqueue(new WeaponPacket()
                {
                    Type = SubPacket.EFirearmSubPacketType.LeftStanceChange,
                    SubPacket =  leftStanceChangePacket
                });
            } 
        }

        [PatchPostfix]
        public static void Postfix()
        {
            if (Config.EnableShoulderSwap.Value)
            {
                TransitionShoulder(StanceController.IsLeftShoulder);
            }
        }
    }
}
using System.Reflection;
using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class CanTurnValvePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InteractionZone).GetMethod("CanTurnValve",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPostfix]
    public static void Postfix(InteractionZone __instance, InteractableGroupComponent ____groupParent, EInteractableState nextState)
    {
        bool completed = ____groupParent.ComplatedSteps >= __instance.InteractableData.CompletionStep;
        bool isSameState = __instance.State == nextState;



        if (!completed || isSameState)
        {


            var packet = new RealismCanTurnValvePacket()
            {
                NextState = nextState,
                Path = __instance.transform.GetFullPath()
            };

            Plugin.REAL_Logger.LogInfo(
                $"Broadcasting can turn valve packet: {packet.Path}, with next state {packet.NextState}");

            if (FikaBackendUtils.IsServer)
            {
                Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
            }
            else
            {
                Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }

        }
    }
}
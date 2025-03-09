using System.Reflection;
using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class InteractableStateChangePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InteractableGroupComponent).GetMethod("HandleValveStateChanged",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPostfix]
    public static void Patch(InteractableGroupComponent __instance, InteractionZone interactable,
        EInteractableState newState)
    {
        

        var packet = new RealismInteractablePacket()
        {
            Path = interactable.transform.GetFullPath(),
            InteractableType = interactable.InteractableData.InteractionType,
            InteractableState = newState,
        };
    
        Plugin.REAL_Logger.LogInfo($"Broadcasting packet: {packet.Path}, with state { packet.InteractableState}");
        
        if (FikaBackendUtils.IsServer)
        {
            // broadcast to all clients
            Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
        }
        else
        {
            // as a client, send to the host
            Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
        }

    }
}
using System.Reflection;
using Comfort.Common;
using EFT;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.Audio.Packets;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RealismModSync.Audio.Patches;

public class PlayGasAnalyserClipPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(RealismAudioControllerComponent).GetMethod(
            nameof(RealismAudioControllerComponent.PlayGasAnalyserClips), BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    public static void Patch(RealismAudioControllerComponent __instance, AudioSource ____gasAnalyserSource)
    {
        CoopHandler.TryGetCoopHandler(out var coopHandler);
        if (coopHandler == null)
        {
            Plugin.REAL_Logger.LogInfo($"CoopHandler is null in Geiger Clip Patch");
            return;
        }
        
        if (____gasAnalyserSource.clip == null)
        {
            Plugin.REAL_Logger.LogInfo($"PlayGasAnalyserClip Patch: no gas anaylzer clip");
            return;
        }
        
        var packet = new RealismAudioPacket()
        {
            NetID = coopHandler.MyPlayer.NetId,
            Clip = ____gasAnalyserSource.clip.name,
            Volume = ____gasAnalyserSource.volume,
            DeviceType = RealismDeviceType.GasAnalyzer
        };

        if (FikaBackendUtils.IsServer)
        {
            Plugin.REAL_Logger.LogInfo($"Sending {____gasAnalyserSource.clip.name} as server");
            Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
        }
        else
        {
            Plugin.REAL_Logger.LogInfo($"Sending {____gasAnalyserSource.clip.name} as client");
            Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
        }
    }
}
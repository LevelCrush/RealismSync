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
    public static void Patch(RealismAudioControllerComponent __instance, ref AudioSource ____gasAnalyserSource)
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
        
        // recreate how realism handles this.
        // technically I dont need volumeModi.
        float volumeModi = 1f;
        string clip = RealismMod.Plugin.RealismAudioControllerComponent.GetGasAnalsyerClip(HazardTracker.BaseTotalToxicityRate, out volumeModi);
        if (clip == null)
        {
            Plugin.REAL_Logger.LogInfo($"Could not detect a suitable gas analyzer clip");
            return;
        }
        
        //float volume = _muteGasAnalyser ? 0f : GetDeviceVolume(GEIGER_VOLUME, volumeModi);
        var packet = new RealismAudioPacket()
        {
            NetID = coopHandler.MyPlayer.NetId,
            Clip = clip,
            Volume = volumeModi,
            DeviceType = RealismDeviceType.GasAnalyzer
        };

        if (FikaBackendUtils.IsServer)
        {
            Plugin.REAL_Logger.LogInfo($"Sending {clip} as server with volume {____gasAnalyserSource.volume} ");
            Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
        }
        else
        {
            Plugin.REAL_Logger.LogInfo($"Sending {clip} as client with volume {____gasAnalyserSource.volume}");
            Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
        }
    }
}
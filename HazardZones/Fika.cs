﻿using System.Collections.Concurrent;
using System.Threading.Tasks;
using ChartAndGraph;
using Comfort.Common;
using EFT;
using Fika.Core.Coop.GameMode;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using RealismModSync.StanceReplication.Components;
using RealismModSync.StanceReplication.Packets;

namespace RealismModSync.HazardZones;

public static class Fika
{

    public static void Register()
    {
        FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(NetworkManagerCreated);
        FikaEventDispatcher.SubscribeEvent<FikaGameCreatedEvent>(GameWorldStarted);
    }

    private static void NetworkManagerCreated(FikaNetworkManagerCreatedEvent @event)
    {
        switch (@event.Manager)
        {
            case FikaClient client:
                client.RegisterPacket<RealismHazardPacket>(HandleHazardPacket);
                client.RegisterPacket<RealismLootPacket>(HandleLootPacket);
                client.RegisterPacket<RealismAssetPacket>(HandleAssetPacket);
                client.RegisterPacket<RealismGasEventPacket>(HandleGasEvent);
                client.RegisterPacket<RealismMapRadPacket>(HandleMapRadPacket);
                break;
        }
    }

    private static void HandleGasEvent(RealismGasEventPacket packet)
    {
        // mirror what Realism Mod is doing
        Plugin.REAL_Logger.LogInfo("Starting Gas Event");
        Core.DoMapGasEvent = true;
        Player player = RealismMod.Utils.GetYourPlayer();
        AudioController.CreateAmbientAudioPlayer(player, player.gameObject.transform, RealismMod.Plugin.GasEventAudioClips, volume: 1.2f, minDelayBeforePlayback: 60f); //spooky short playback
        AudioController.CreateAmbientAudioPlayer(player, player.gameObject.transform, RealismMod.Plugin.GasEventLongAudioClips, true, 5f, 30f, 0.2f, 55f, 65f, minDelayBeforePlayback: 0f); //long ambient
    }

    private static void HandleMapRadPacket(RealismMapRadPacket packet)
    {
        // mirror what Realism Mod is doing 
        Plugin.REAL_Logger.LogInfo($"Starting Map Radiation");
        Core.DoMapRad = true;
        Player player = RealismMod.Utils.GetYourPlayer();
        RealismMod.AudioController.CreateAmbientAudioPlayer(player, player.gameObject.transform, RealismMod.Plugin.RadEventAudioClips, volume: 1f, minDelayBeforePlayback: 60f); //thunder
    }

    private static void HandleAssetPacket(RealismAssetPacket packet)
    {
        Plugin.REAL_Logger.LogInfo($"Received asset packet: {packet.AssetName} at position {packet.Position} rotation {packet.Rotation}");
        Core.LoadAsset(packet.AssetName, packet.Position, packet.Rotation);
    }

    private static void HandleLootPacket(RealismLootPacket packet)
    {
        Plugin.REAL_Logger.LogInfo($"Received loot packet: {packet.MongoID} at {packet.Position}|{packet.Rotation} of template {packet.TemplateId}");
        Core.LoadLooseLoot(packet.Position, packet.Rotation, packet.TemplateId, packet.MongoID);
    }

    private static void HandleHazardPacket(RealismHazardPacket packet)
    {
        
        Plugin.REAL_Logger.LogInfo($"Hazard Packet received. Zone: {packet.ZoneKey} Type: {(int)packet.ZoneType}");

        Core.HazardGroups.TryGetValue(packet.ZoneKey, out var hazardGroup);
        if (hazardGroup == null)
        {
            Plugin.REAL_Logger.LogError($"Hazard group {packet.ZoneKey} not found. Can't spawn.");
            return;
        }

        Plugin.REAL_Logger.LogInfo($"Updating cache zone results for: {packet.ZoneKey}");
        Core.ZoneResults.AddOrUpdate(packet.ZoneKey, true, (key, oldValue) => true);
        
        // immediately spawn in the zones when the packet is received
        switch (packet.ZoneType)
        {
            case EZoneType.Gas:
            case EZoneType.GasAssets:
                ZoneSpawner.CreateZone<GasZone>(hazardGroup, packet.ZoneType);
                break;
            case EZoneType.Radiation:
            case EZoneType.RadAssets:
                ZoneSpawner.CreateZone<RadiationZone>(hazardGroup, packet.ZoneType);
                break;
            case EZoneType.Interactable:
                ZoneSpawner.CreateZone<InteractionZone>(hazardGroup, EZoneType.Interactable);
                break;
            case EZoneType.SafeZone:
                ZoneSpawner.CreateZone<LabsSafeZone>(hazardGroup, EZoneType.SafeZone);
                break;
            case EZoneType.Quest:
                ZoneSpawner.CreateZone<QuestZone>(hazardGroup, EZoneType.Quest);
                break;
            default:
                Plugin.REAL_Logger.LogWarning($"Unknown Zone Type: {(int)packet.ZoneType} for key {packet.ZoneKey}");
                break;
            
        }
    }

    private static void LoadZones(EZoneType zoneType, string map)
    {
        Plugin.REAL_Logger.LogInfo($"Loading Realism zones for {map} and type {(int)zoneType}");
        var zones = ZoneData.GetZones(zoneType, map);
        foreach (var zone in zones)
        {
            var zoneKey = Core.GenerateZoneKey(zone, zoneType);
            Plugin.REAL_Logger.LogInfo($"Zone: {zoneKey} is cached");
            Core.HazardGroups.AddOrUpdate(zoneKey, zone, (s, group) => zone);
        }
    }
    
    private static void GameWorldStarted(FikaGameCreatedEvent @event)
    {
        Plugin.REAL_Logger.LogInfo("Initializing Zone Results and loading Zone data");
        // handle any initiation when the game world starts here
        if (Core.ZoneResults == null)
        {
            Core.ZoneResults = new ConcurrentDictionary<string, bool>();
        }
        
        Core.ZoneResults.Clear();

        
        if (Core.ZoneWillSpawn == null)
        {
            Core.ZoneWillSpawn = new ConcurrentDictionary<string, bool>();
        }
        
        Core.ZoneWillSpawn.Clear();

        // make sure this is set to false. This only impacts Fika Clients realistically
        Core.DoMapGasEvent = false;
        Core.DoMapRad = false;

        var map = ((CoopGame)@event.Game).Location_0.Id.ToLower();
        
        LoadZones(EZoneType.Gas, map);
        LoadZones(EZoneType.GasAssets, map);
        LoadZones(EZoneType.Radiation, map);
        LoadZones(EZoneType.RadAssets, map);
        LoadZones(EZoneType.Interactable, map);
        LoadZones(EZoneType.SafeZone, map);
        LoadZones(EZoneType.Quest, map);

    }

    
}
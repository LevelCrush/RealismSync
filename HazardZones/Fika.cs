using System.Collections.Concurrent;
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
                break;
        }
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
                ZoneSpawner.CreateZone<GasZone>(hazardGroup, EZoneType.Gas);
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
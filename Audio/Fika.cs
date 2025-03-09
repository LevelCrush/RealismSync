using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using ChartAndGraph;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using Fika.Core.Coop.GameMode;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using LiteNetLib;
using RealismMod;
using RealismModSync.HazardZones.Packets;
using RealismModSync.StanceReplication.Components;
using RealismModSync.StanceReplication.Packets;
using UnityEngine;

namespace RealismModSync.Audio;

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
            case FikaServer server:
                // todo fill
                break;
            case FikaClient client:
                // todo fill
                break;
        }
        
    }

    
    private static void GameWorldStarted(FikaGameCreatedEvent @event)
    {
        Plugin.REAL_Logger.LogInfo("Initializing Audio");

    }

    
}
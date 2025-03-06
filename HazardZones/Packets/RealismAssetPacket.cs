using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace RealismModSync.HazardZones.Packets;

public class RealismAssetPacket: INetSerializable
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string AssetName;
    
    public void Deserialize(NetDataReader reader)
    {
        AssetName = reader.GetString();
        Position = reader.GetVector3();
        Rotation = reader.GetVector3();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(AssetName);
        writer.Put(Position);
        writer.Put(Rotation);
    }
}
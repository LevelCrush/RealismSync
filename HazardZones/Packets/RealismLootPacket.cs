using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace RealismModSync.HazardZones.Packets;

public class RealismLootPacket: INetSerializable
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string TemplateId;
    public string MongoID;

    public void Deserialize(NetDataReader reader)
    {
        MongoID = reader.GetString();
        TemplateId = reader.GetString();
        Position = reader.GetVector3();
        Rotation = reader.GetVector3();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(MongoID);
        writer.Put(TemplateId);
        writer.Put(Position);
        writer.Put(Rotation);
    }
}
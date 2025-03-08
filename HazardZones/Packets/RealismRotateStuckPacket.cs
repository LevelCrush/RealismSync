using System.Diagnostics;
using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismRotateStuckPacket : INetSerializable
{
    public string ZoneKey;
    public string InteractionZoneName;
    public float Direction;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(ZoneKey);
        writer.Put(InteractionZoneName);
        writer.Put(Direction);
    }

    public void Deserialize(NetDataReader reader)
    {
        ZoneKey = reader.GetString();
        InteractionZoneName = reader.GetString();
        Direction = reader.GetFloat();
    }
    
}
using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismMapRadPacket : INetSerializable
{
    public void Serialize(NetDataWriter writer)
    {
        // literally nothing needed to serialize
        // we only send this packet when map rad is going on 
    }

    public void Deserialize(NetDataReader reader)
    {
        // literally nothing here to grab
    }
}
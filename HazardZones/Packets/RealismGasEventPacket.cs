using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismGasEventPacket : INetSerializable
{
    public void Serialize(NetDataWriter writer)
    {
        // literally nothing needed to serialize
        // we only send this packet when there is a gas event going on
    }

    public void Deserialize(NetDataReader reader)
    {
        // literally nothing here to grab
    }
}
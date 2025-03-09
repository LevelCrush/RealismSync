using System.Diagnostics;
using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismRotateStuckPacket : INetSerializable
{

    public float Direction;
    public string Path;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Path);
        writer.Put(Direction);
    }

    public void Deserialize(NetDataReader reader)
    {
        Path = reader.GetString();
        Direction = reader.GetFloat();
    }
    
}
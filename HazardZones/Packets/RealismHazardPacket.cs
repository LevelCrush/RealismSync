using LiteNetLib.Utils;
using RealismMod;

namespace RealismModSync.HazardZones.Packets;

public class RealismHazardPacket : INetSerializable
{
    public string ZoneKey;
    public EZoneType ZoneType;
    
    public void Deserialize(NetDataReader reader)
    {
        ZoneKey = reader.GetString();
        ZoneType = (EZoneType)reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(ZoneKey);
        writer.Put((int)ZoneType);
    }
}
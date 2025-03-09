using System.Diagnostics;
using LiteNetLib.Utils;
using RealismMod;

namespace RealismModSync.HazardZones.Packets;

public class RealismCanTurnValvePacket : INetSerializable
{

    public EInteractableState NextState;
    public string Path;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Path);
        writer.Put((int)NextState);
    }

    public void Deserialize(NetDataReader reader)
    {
        Path = reader.GetString();
        NextState = (EInteractableState)reader.GetInt();
    }
    
}
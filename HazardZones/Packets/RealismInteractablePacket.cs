using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismInteractablePacket : INetSerializable
{
    public RealismMod.EInteractableState InteractableState;
    public RealismMod.EIneractableType InteractableType;
    public string Path;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Path);
        writer.Put((int)InteractableType);
        writer.Put((int)InteractableState);
    }

    public void Deserialize(NetDataReader reader)
    {
        Path = reader.GetString();
        InteractableType = (RealismMod.EIneractableType)reader.GetInt();
        InteractableState = (RealismMod.EInteractableState)reader.GetInt();
    }
}
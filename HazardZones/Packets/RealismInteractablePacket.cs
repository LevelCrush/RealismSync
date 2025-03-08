using LiteNetLib.Utils;

namespace RealismModSync.HazardZones.Packets;

public class RealismInteractablePacket : INetSerializable
{
    public RealismMod.EInteractableState InteractableState;
    public RealismMod.EIneractableType InteractableType;
    public RealismMod.EIneractableAction InteractableAction;
    public string ZoneKey;
    public string InteractionZoneName;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(ZoneKey);
        writer.Put(InteractionZoneName);
        writer.Put((int)InteractableType);
        writer.Put((int)InteractableState);
        writer.Put((int)InteractableAction);
    }

    public void Deserialize(NetDataReader reader)
    {
        ZoneKey = reader.GetString();
        InteractionZoneName = reader.GetString();
        InteractableType = (RealismMod.EIneractableType)reader.GetInt();
        InteractableState = (RealismMod.EInteractableState)reader.GetInt();
        InteractableAction = (RealismMod.EIneractableAction)reader.GetInt();
    }
}
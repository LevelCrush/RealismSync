using LiteNetLib.Utils;

namespace RealismModSync.Audio.Packets;

public class RealismAudioPacket : INetSerializable
{
    public string Clip;
    public float Volume;
    public RealismDeviceType DeviceType;
    public int NetID;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetID);
        writer.Put(Clip);
        writer.Put(Volume);
        writer.Put((int)DeviceType);
    }
    
    public void Deserialize(NetDataReader reader)
    {
        NetID = reader.GetInt();
        Clip = reader.GetString();
        Volume = reader.GetFloat();
        DeviceType = (RealismDeviceType)reader.GetInt();
    }
}
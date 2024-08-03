using System.Net;
using System.Net.NetworkInformation;

namespace PetiteNet;

public enum FrameType : ushort
{
    IPv4 = 0x0800,
    ARP = 0x0806,
    IPv6 = 0x86DD
}

public record Frame(PhysicalAddress Destination, PhysicalAddress Source, FrameType Type)
{
    public static Frame ReadFromStream(BinaryReader reader)
    {
        var destination = Helpers.ReadPhysicalAddressFromStream(reader);
        var source = Helpers.ReadPhysicalAddressFromStream(reader);
        var type = (FrameType)IPAddress.NetworkToHostOrder(reader.ReadInt16());
        return new Frame(destination, source, type);
    }

    public void WriteToStream(BinaryWriter writer)
    {
        writer.Write(Destination.GetAddressBytes());
        writer.Write(Source.GetAddressBytes());
        writer.WriteNetworkOrder((ushort)Type);
    }
}
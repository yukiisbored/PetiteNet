using System.Net;
using System.Net.NetworkInformation;

namespace PetiteNet;

public enum ARPOperation : ushort
{
    Request = 0x1,
    Reply = 0x2
}

public enum ARPHardwareType : ushort
{
    Ethernet = 0x1
}

public enum ARPProtocolType : ushort
{
    IPv4 = 0x0800
}

public record ARP(
    ARPHardwareType HardwareType,
    ARPProtocolType ProtocolType,
    byte HardwareAddressLength,
    byte ProtocolAddressLength,
    ARPOperation Operation,
    PhysicalAddress SenderHardwareAddress,
    IPAddress SenderProtocolAddress,
    PhysicalAddress TargetHardwareAddress,
    IPAddress TargetProtocolAddress)
{
    public static ARP ReadFromStream(BinaryReader reader)
    {
        var hardwareType = (ARPHardwareType)IPAddress.NetworkToHostOrder(reader.ReadInt16());
        var protocolType = (ARPProtocolType)IPAddress.NetworkToHostOrder(reader.ReadInt16());
        var hardwareAddressLength = reader.ReadByte();
        var protocolAddressLength = reader.ReadByte();
        var arpOperation = (ARPOperation)IPAddress.NetworkToHostOrder(reader.ReadInt16());
        var senderHardwareAddress = Helpers.ReadPhysicalAddressFromStream(reader);
        var senderProtocolAddress = Helpers.ReadIPAddressFromStream(reader);
        var targetHardwareAddress = Helpers.ReadPhysicalAddressFromStream(reader);
        var targetProtocolAddress = Helpers.ReadIPAddressFromStream(reader);
        return new ARP(
            hardwareType, protocolType,
            hardwareAddressLength, protocolAddressLength,
            arpOperation,
            senderHardwareAddress, senderProtocolAddress,
            targetHardwareAddress, targetProtocolAddress);
    }

    public void WriteToStream(BinaryWriter writer)
    {
        writer.WriteNetworkOrder((ushort)HardwareType);
        writer.WriteNetworkOrder((ushort)ProtocolType);
        writer.Write(HardwareAddressLength);
        writer.Write(ProtocolAddressLength);
        writer.WriteNetworkOrder((ushort)Operation);
        writer.Write(SenderHardwareAddress.GetAddressBytes());
        writer.Write(SenderProtocolAddress.GetAddressBytes());
        writer.Write(TargetHardwareAddress.GetAddressBytes());
        writer.Write(TargetProtocolAddress.GetAddressBytes());
    }
}
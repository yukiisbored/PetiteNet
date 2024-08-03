using System.Net;
using System.Net.NetworkInformation;

namespace PetiteNet;

public class Interface
{
    public readonly IPAddress IPAddress;
    public readonly IPNetwork IPNetwork;
    public readonly PhysicalAddress PhysicalAddress;

    public readonly BinaryWriter Writer;

    public Interface(IPAddress ipAddress, IPNetwork ipNetwork, PhysicalAddress physicalAddress, BinaryWriter writer)
    {
        if (!ipNetwork.Contains(ipAddress))
            throw new ArgumentException("IP address must be in the given network", nameof(ipAddress));

        IPAddress = ipAddress;
        IPNetwork = ipNetwork;
        PhysicalAddress = physicalAddress;
        Writer = writer;
    }


    public void Receive(BinaryReader reader)
    {
        var frame = Frame.ReadFromStream(reader);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(frame);

        switch (frame.Type)
        {
            case FrameType.ARP:
                var arp = ARP.ReadFromStream(reader);
                Console.WriteLine(arp);
                Console.ForegroundColor = ConsoleColor.White;
                HandleARP(arp);
                break;

            case FrameType.IPv4:
            case FrameType.IPv6:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleARP(ARP arp)
    {
        if (arp.Operation != ARPOperation.Request)
            return;

        if (!arp.TargetProtocolAddress.Equals(IPAddress))
            return;

        var frame = new Frame(arp.SenderHardwareAddress, PhysicalAddress, FrameType.ARP);
        var reply = arp with
        {
            Operation = ARPOperation.Reply,
            SenderHardwareAddress = PhysicalAddress,
            SenderProtocolAddress = IPAddress,
            TargetHardwareAddress = arp.SenderHardwareAddress,
            TargetProtocolAddress = arp.SenderProtocolAddress
        };

        Console.WriteLine(frame);
        Console.WriteLine(reply);

        frame.WriteToStream(Writer);
        reply.WriteToStream(Writer);
    }
}
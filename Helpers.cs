using System.Net;
using System.Net.NetworkInformation;

namespace PetiteNet;

public static class Helpers
{
    public static PhysicalAddress ReadPhysicalAddressFromStream(BinaryReader reader)
    {
        var address = reader.ReadBytes(6);
        return new PhysicalAddress(address);
    }

    public static IPAddress ReadIPAddressFromStream(BinaryReader reader)
    {
        var address = reader.ReadBytes(4);
        return new IPAddress(address);
    }
}
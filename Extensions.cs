using System.Buffers.Binary;

namespace PetiteNet;

public static class Extensions
{
    public static void WriteNetworkOrder(this BinaryWriter writer, ushort value)
    {
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(bytes, value);
        writer.Write(bytes);
    }
}
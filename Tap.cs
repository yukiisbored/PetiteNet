using System.Runtime.InteropServices;
using System.Text;

namespace PetiteNet;

public static partial class Tap
{
    private const int IFF_TAP = 0x0002;
    private const int IFF_NO_PI = 0x1000;
    private const int TUNSETIFF = 0x400454CA;

    [LibraryImport("libc.so.6", SetLastError = true)]
    private static partial int ioctl(int fd, int request, IntPtr argp);

    public static FileStream Open(string name)
    {
        var tap = new FileStream("/dev/net/tun", FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        var ifr = new byte[16 + 2];
        var nameBytes = Encoding.UTF8.GetBytes(name);
        Array.Copy(nameBytes, ifr, Math.Min(nameBytes.Length, 16));
        BitConverter.GetBytes((short)(IFF_TAP | IFF_NO_PI)).CopyTo(ifr, 16);

        var ifrHandle = GCHandle.Alloc(ifr, GCHandleType.Pinned);
        try
        {
            if (ioctl(tap.SafeFileHandle.DangerousGetHandle().ToInt32(), TUNSETIFF, ifrHandle.AddrOfPinnedObject()) < 0)
                throw new InvalidOperationException("Failed to configure TAP device.");
        }
        finally
        {
            ifrHandle.Free();
        }

        return tap;
    }
}
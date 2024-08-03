using System.Net;
using System.Net.NetworkInformation;
using PetiteNet;

var physicalAddress = PhysicalAddress.Parse("00:11:22:33:44:55");
var ipAddress = IPAddress.Parse("10.0.0.128");
var ipNetwork = new IPNetwork(IPAddress.Parse("10.0.0.0"), 8);

using var tap = Tap.Open("tap0");

var intf = new Interface(ipAddress, ipNetwork, physicalAddress, new BinaryWriter(tap));

var buffer = new byte[1500];

while (tap.Read(buffer) > 0)
{
    Stream stream = new MemoryStream(buffer);
    var reader = new BinaryReader(stream);
    intf.Receive(reader);
}
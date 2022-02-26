using System.Net;

namespace AmeDhcpServer.Core.Entities;

public class NetworkConfiguration : AggregateRoot<string>
{
    public string Id { get; }
    public IPAddress Gateway1 { get; }
    public IPAddress? Gateway2 { get; }
    public IPAddress Dhcp { get; }
    public IPAddress SubnetMask { get; }
    public IPAddress Dns1 { get; }
    public IPAddress Dns2 { get; }

    public NetworkConfiguration(string id, IPAddress gateway1, IPAddress subnetMask, IPAddress dns1, IPAddress dns2, IPAddress dhcp)
    {
        Id = id;
        Gateway1 = gateway1;
        SubnetMask = subnetMask;
        Dns1 = dns1;
        Dns2 = dns2;
        Dhcp = dhcp;
    }
}
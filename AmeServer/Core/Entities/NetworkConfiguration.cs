using System.Net;

namespace AmeServer.Core.Entities;

// ReSharper disable once ClassNeverInstantiated.Global
public class NetworkConfiguration : AggregateRoot<string>
{
    public IPAddress[] Gateways { get; }
    public IPAddress Dhcp { get; }
    public IPAddress SubnetMask { get; }
    public IPAddress[] Dns { get; }
    public IPAddress PoolMin { get; }
    public IPAddress PoolMax { get; }
    public int Priority { get; }

    public NetworkConfiguration(string id, IPAddress[] gateways, IPAddress subnetMask, IPAddress[] dns, IPAddress dhcp, IPAddress poolMin, IPAddress poolMax, int priority)
    {
        Id = id;
        Gateways = gateways;
        SubnetMask = subnetMask;
        Dns = dns;
        Dhcp = dhcp;
        PoolMin = poolMin;
        Priority = priority;
        PoolMax = poolMax;
    }
}
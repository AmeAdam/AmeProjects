using System.Net;
using System.Net.NetworkInformation;
using AmeServer.Core.Events;

namespace AmeServer.Core.Entities;

public class NetworkDevice : AggregateRoot<PhysicalAddress>
{
    public string Name { get; }
    public string? PresentedName { get; private set; }
    public IPAddress ClientAddress { get; }
    public bool Permanent { get; }
    public DateTime UpdateTime { get; private set; }
    public int LeaseTimeSeconds { get; private set;}
    public NetworkConfiguration? PreferredConfiguration { get; private set; }
    public DhcpState State { get; private set; }

    private NetworkDevice()
    {
    }

    public NetworkDevice(PhysicalAddress id, string name, IPAddress clientAddress, bool permanent, NetworkConfiguration? preferredConfiguration)
    {
        Id = id;
        Name = name;
        ClientAddress = clientAddress;
        Permanent = permanent;
        UpdateTime = DateTime.Now;
        LeaseTimeSeconds = (int)TimeSpan.FromDays(1).TotalSeconds;
        PreferredConfiguration = preferredConfiguration;
        State = DhcpState.Unknown;
    }

    public void Discover(DhcpMessage discoverMessage)
    {
        PresentedName = discoverMessage.HostName ?? PresentedName;

        if (State == DhcpState.Offered && UpdateTime.AddSeconds(10) > DateTime.Now)
        {
            return;
        }

        if (State == DhcpState.Forbidden)
        {
            AddEvent(new DhcpRequestRejectedEvent(this, discoverMessage));
        }
        else
        {
            UpdateTime = DateTime.Now;
            State = DhcpState.Offered;
            AddEvent(new DhcpRequestAcceptedEvent(this, discoverMessage));
        }
    }

    public void Request(DhcpMessage requestMessage)
    {
        PresentedName = requestMessage.HostName ?? PresentedName;
        if (State == DhcpState.Forbidden)
        {
            AddEvent(new DhcpRequestRejectedEvent(this, requestMessage));
        }

        var clientOffer = requestMessage.ClientIPAddress;

        if (requestMessage.Options.TryGetValue(DhcpMessage.DhcpOptionCode.RequestedIpAddress,
                out var requestedIpAddress))
        {
            clientOffer = new IPAddress(requestedIpAddress);
        }

        if (clientOffer.Equals(ClientAddress))
        {
            State = DhcpState.Accepted;
            AddEvent(new DhcpRequestAcceptedEvent(this, requestMessage));
        }
        else
            AddEvent(new DhcpRequestRejectedEvent(this, requestMessage));

    }

    public enum DhcpState
    {
        Forbidden,
        Unknown,
        Offered,
        Accepted
    }
}
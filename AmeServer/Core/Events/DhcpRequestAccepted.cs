using AmeServer.Core.Entities;

namespace AmeServer.Core.Events;

public class DhcpRequestAcceptedEvent : IDomainEvent
{
    public NetworkDevice NetworkDevice { get; }
    public DhcpMessage RequestMessage { get; }

    public DhcpRequestAcceptedEvent(NetworkDevice networkDevice, DhcpMessage requestMessage)
    {
        NetworkDevice = networkDevice;
        RequestMessage = requestMessage;
    }
}
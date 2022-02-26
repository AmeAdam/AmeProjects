using AmeDhcpServer.Core.Entities;

namespace AmeDhcpServer.Core.Events;

public class DhcpRequestFromUnknownDeviceEvent : IDomainEvent
{
    public NetworkDevice NetworkDevice { get; }
    public DhcpMessage RequestMessage { get; }

    public DhcpRequestFromUnknownDeviceEvent(NetworkDevice networkDevice, DhcpMessage requestMessage)
    {
        NetworkDevice = networkDevice;
        RequestMessage = requestMessage;
    }
}
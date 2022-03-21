using System.Net;
using AmeServer.Utils;
using AmeServer.Application.Services;
using AmeServer.Core;
using AmeServer.Core.Events;
using MediatR;

namespace AmeServer.Application.EventsHandlers;

public class DhcpRequestEventHandler : 
    INotificationHandler<DhcpRequestAcceptedEvent>,
    INotificationHandler<DhcpRequestRejectedEvent>
{
    private readonly IUdpSender udpSender;

    public DhcpRequestEventHandler(IUdpSender udpSender)
    {
        this.udpSender = udpSender;
    }

    public async Task Handle(DhcpRequestAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var networkConfiguration = notification.NetworkDevice.PreferredConfiguration;

        if (networkConfiguration != null)
        {
            var message =
                notification.RequestMessage.CreateReplay(notification.NetworkDevice.ClientAddress,
                    networkConfiguration.Dhcp);

            message
                .AddMessageType(notification.NetworkDevice.State)
                .AddSubnetMask(networkConfiguration.SubnetMask)
                .AddRouter(networkConfiguration.Gateways)
                .AddDns(networkConfiguration.Dns)
                .AddNetBiosOverTcpIpNodeType(8)
                .AddAddressTime(notification.NetworkDevice.LeaseTimeSeconds)
                .AddDhcp(networkConfiguration.Dhcp);

            await udpSender.Send(message);
        }
    }

    public async Task Handle(DhcpRequestRejectedEvent notification, CancellationToken cancellationToken)
    {
        var networkConfiguration = notification.NetworkDevice.PreferredConfiguration;

        var message = notification.RequestMessage.CreateReplay(notification.NetworkDevice.ClientAddress, networkConfiguration?.Dhcp ?? IPAddress.None);
        message.AddMessageType(DhcpMessage.DhcpMessageType.Nak);
        
        await udpSender.Send(message);
    }
}
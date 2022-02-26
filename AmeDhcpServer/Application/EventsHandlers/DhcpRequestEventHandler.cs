using AmeDhcpServer.Application.Services;
using AmeDhcpServer.Core;
using AmeDhcpServer.Core.Entities;
using AmeDhcpServer.Core.Events;
using AmeDhcpServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AmeDhcpServer.Application.EventsHandlers;

public class DhcpRequestEventHandler : 
    INotificationHandler<DhcpRequestAcceptedEvent>,
    INotificationHandler<DhcpRequestRejectedEvent>
{
    private readonly IUdpSender udpSender;
    private readonly ApplicationContext context;

    public DhcpRequestEventHandler(IUdpSender udpSender, ApplicationContext context)
    {
        this.udpSender = udpSender;
        this.context = context;
    }

    private async Task<NetworkConfiguration> GetNetworkConfiguration(string? preferConfigurationName,
        CancellationToken cancellationToken)
    {
        var networkConfiguration =
            await context.NetworkConfigurations.FirstOrDefaultAsync(g => g.Id == preferConfigurationName, cancellationToken: cancellationToken) ??
            await context.NetworkConfigurations.FirstAsync(cancellationToken);
        return networkConfiguration;
    }

    public async Task Handle(DhcpRequestAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var networkConfiguration =
            await GetNetworkConfiguration(notification.NetworkDevice.PreferredNetworkConfiguration, cancellationToken);

        var message =
            notification.RequestMessage.CreateReplay(notification.NetworkDevice.ClientAddress,
                networkConfiguration.Dhcp);

        if (notification.NetworkDevice.State == NetworkDevice.DhcpState.Offered)
            message.AddOption(DhcpMessage.DhcpOptionCode.DhcpMessageType, DhcpMessage.DhcpMessageType.Offer);
        if (notification.NetworkDevice.State == NetworkDevice.DhcpState.Accepted)
            message.AddOption(DhcpMessage.DhcpOptionCode.DhcpMessageType, DhcpMessage.DhcpMessageType.Ack);

        message.AddOption(DhcpMessage.DhcpOptionCode.SubnetMask, networkConfiguration.SubnetMask);
        message.AddOption(DhcpMessage.DhcpOptionCode.Router, networkConfiguration.Gateway1);
        message.AddOption(DhcpMessage.DhcpOptionCode.NetBIOSOverTCPIPNodeType, new byte[] { 8 });
        message.AddOption(DhcpMessage.DhcpOptionCode.DomainNameServer, networkConfiguration.Dns1);
        message.AddOption(DhcpMessage.DhcpOptionCode.AddressTime, notification.NetworkDevice.LeaseTimeSeconds, true);
        message.AddOption(DhcpMessage.DhcpOptionCode.DhcpAddress, networkConfiguration.Dhcp);

        await udpSender.Send(message);
    }

    public async Task Handle(DhcpRequestRejectedEvent notification, CancellationToken cancellationToken)
    {
        var networkConfiguration =
            await GetNetworkConfiguration(notification.NetworkDevice.PreferredNetworkConfiguration, cancellationToken);

        var message = notification.RequestMessage.CreateReplay(notification.NetworkDevice.ClientAddress, networkConfiguration.Dhcp);
        message.AddOption(DhcpMessage.DhcpOptionCode.DhcpMessageType, DhcpMessage.DhcpMessageType.Nak);
        
        await udpSender.Send(message);
    }
}
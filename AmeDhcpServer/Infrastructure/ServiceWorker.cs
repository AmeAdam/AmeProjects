using System.Net.Sockets;
using System.Reactive.Linq;
using AmeDhcpServer.Application.Commands;
using AmeDhcpServer.Application.Services;
using AmeDhcpServer.Utils;
using MediatR;

namespace AmeDhcpServer.Infrastructure;

public class ServiceWorker : IHostedService
{
    private IDisposable? udpSubscription;
    private readonly IUdpSender udp;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<ServiceWorker> log;

    public ServiceWorker(IUdpSender udp, IServiceScopeFactory serviceScopeFactory, ILogger<ServiceWorker> log)
    {
        this.udp = udp;
        this.serviceScopeFactory = serviceScopeFactory;
        this.log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        udpSubscription = udp.UdpStream()
            .Do(
                data => ProcessRequest(data).Wait(),
                error => { log.LogError(error,"UDP RX stream error");})
            .Retry()
            .Subscribe();

        log.LogInformation("DHCP Started");
        return Task.CompletedTask;
    }

    private void UdpError(Exception ex)
    {
        log.LogError(ex, "UDP subscription failed");
    }
    
    private async Task ProcessRequest(UdpReceiveResult message)
    {
        try
        {
            var dhcpMessage = DhcpPacketSerializer.Deserialize(message.Buffer).ToMessage();
            log.LogInformation("Received packet from {Host}", dhcpMessage.HostName);

            using var scope = serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new DhcpRequest.Command(dhcpMessage));
        }
        catch (Exception ex)
        {
            log.LogError(ex,"");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        udpSubscription?.Dispose();
        return Task.CompletedTask;
    }
    

}
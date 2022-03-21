using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using AmeServer.Utils;
using AmeServer.Application.Services;
using AmeServer.Core;

namespace AmeServer.Infrastructure;

public class UdpService : IUdpSender, IDisposable
{
    private readonly ILogger<UdpService> logger;
    private readonly IPEndPoint endPoint;
    private readonly UdpClient client = new (67);

    public UdpService(ILogger<UdpService> logger)
    {
        this.logger = logger;
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        endPoint = new IPEndPoint(IPAddress.Broadcast, 68);
    }

    public IObservable<UdpReceiveResult> UdpStream()
    {
        return Observable.Defer(() => Observable.FromAsync(client.ReceiveAsync)).Repeat();
    }

    public Task Send(DhcpMessage message)
    {
        try
        {
            var data = message.ToBytes();
            client.Send(data, data.Length, endPoint);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending Dhcp reply");
            throw;
        }
    }

    public void Dispose()
    {
        client.Dispose();
    }
}
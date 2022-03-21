using System.Net.Sockets;
using AmeServer.Core;

namespace AmeServer.Application.Services;

public interface IUdpSender
{
    Task Send(DhcpMessage message);
    IObservable<UdpReceiveResult> UdpStream();
}
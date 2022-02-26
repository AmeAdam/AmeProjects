using System.Net.Sockets;
using AmeDhcpServer.Core;

namespace AmeDhcpServer.Application.Services;

public interface IUdpSender
{
    Task Send(DhcpMessage message);
    IObservable<UdpReceiveResult> UdpStream();
}
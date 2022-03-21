using System.Net;
using AmeServer.Core;
using AmeServer.Core.Entities;

namespace AmeServer.Utils;

public static class DhcpMessageExtensions
{
    public static DhcpMessage AddSubnetMask(this DhcpMessage message, IPAddress mask)
    {
        message.Options[DhcpMessage.DhcpOptionCode.SubnetMask] = mask.GetAddressBytes();
        return message;
    }
    
    public static DhcpMessage AddRouter(this DhcpMessage message, params IPAddress[] routers)
    {
        message.Options[DhcpMessage.DhcpOptionCode.Router] = routers.ToBytes();
        return message;
    }
    
    public static DhcpMessage AddDns(this DhcpMessage message, params IPAddress[] dns)
    {
        message.Options[DhcpMessage.DhcpOptionCode.DomainNameServer] = dns.ToBytes();
        return message;
    }
    
    public static DhcpMessage AddNetBiosOverTcpIpNodeType(this DhcpMessage message, byte value)
    {
        message.Options[DhcpMessage.DhcpOptionCode.NetBiosOverTcpIpNodeType] = new[] {value};
        return message;
    }
    
    public static DhcpMessage AddAddressTime(this DhcpMessage message, int seconds)
    {
        var bytes = BitConverter.GetBytes(seconds);
        Array.Reverse(bytes);
        message.Options[DhcpMessage.DhcpOptionCode.AddressTime] = bytes;
        return message;
    }
    
    public static DhcpMessage AddDhcp(this DhcpMessage message, IPAddress dhcp)
    {
        message.Options[DhcpMessage.DhcpOptionCode.DhcpAddress] = dhcp.GetAddressBytes();
        return message;
    }
    
    public static DhcpMessage AddMessageType(this DhcpMessage message, DhcpMessage.DhcpMessageType type)
    {
        message.Options[DhcpMessage.DhcpOptionCode.DhcpMessageType] = new []{(byte)type};
        return message;
    }
    
    public static DhcpMessage AddMessageType(this DhcpMessage message, NetworkDevice.DhcpState type)
    {
        switch (type)
        {
            case NetworkDevice.DhcpState.Accepted:
                return message.AddMessageType(DhcpMessage.DhcpMessageType.Ack);
            case NetworkDevice.DhcpState.Offered:
                return message.AddMessageType(DhcpMessage.DhcpMessageType.Offer);
        }

        return message;
    }

    private static byte[] ToBytes(this IPAddress[] addresses)
    {
        var bytes = addresses.Select(r => r.GetAddressBytes()).Aggregate(new List<byte>(4 * addresses.Length),
            (acc, current) =>
            {
                acc.AddRange(current);
                return acc;
            });
        return bytes.ToArray();
    }
}
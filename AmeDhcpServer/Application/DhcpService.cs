using System.Net;
using System.Net.Sockets;
using AmeDhcpServer.Application.DhcpModel;
using AmeDhcpServer.Core;
using AmeDhcpServer.Utils;

namespace AmeDhcpServer.Application;

public class DhcpService
{
    IPAddress DhcpInterfaceAddress = IPAddress.Parse("192.168.8.3");
    IPAddress Gateway = IPAddress.Parse("192.168.8.1");
    IPAddress SubnetMask = IPAddress.Parse("255.255.255.0");
    IPAddress DnsServer = IPAddress.Parse("192.168.8.1");
    int LeaseTimeSeconds = 3600;
    
    
    private async Task SendReply(DhcpPacket packet)
    {
        try
        {
            var response = packet.GetBytes();
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 68);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

                await client.SendAsync(response, response.Length, endPoint)
                    .ConfigureAwait(false);               
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending Dhcp reply: {ex.Message}");
            throw;
        }
    }

    public async Task Request(DhcpMessage message)
    {
        try
        {
            // Client specified an address they would like
            if (message.Options.ContainsKey(DhcpMessage.DhcpOptionCode.RequestedIpAddress))
            {
                await KeepAddressRequest(message).ConfigureAwait(false);
            }
            else
            {
                var clientAddress = message.ClientIPAddress;

                if (clientAddress.Equals(IPAddress.Parse("0.0.0.0")))
                {
                    // A DHCP REQ should have an address
                    throw new Exception("A DHCP Request must have an address specified");
                }
                
                var expectedAddress = IPAddress.Parse("192.168.8.248");

                if (!expectedAddress.Equals(clientAddress))
                {
                    throw new ApplicationException($"request from {clientAddress}");
                }

                //await LeaseManager.AddLease(clientAddress, message.ClientHardwareAddress, message.HostName)
                //    .ConfigureAwait(false);

                await SendAck(message, clientAddress);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private async Task SendAck(DhcpMessage message, IPAddress clientAddress)
    {
        //Log.Debug($"[ACK] Creating the acknowledgement");

        message.OperationCode = DhcpMessage.DhcpOperation.BootReply;
        message.YourIPAddress = clientAddress;
        message.ServerIPAddress = DhcpInterfaceAddress;

        var optionBuilder = new DhcpOptionBuilder();
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.DhcpMessageType, DhcpMessage.DhcpMessageType.Ack);
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.DhcpAddress, DhcpInterfaceAddress);
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.SubnetMask, SubnetMask);
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.Router, Gateway);
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.DomainNameServer, DnsServer);
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.AddressTime, LeaseTimeSeconds, true);

        var packet = message.ToPacket(optionBuilder.GetBytes());
        await SendReply(packet);
        //Log.Debug($"[ACK] IP Address '{clientAddress}' was sent over '{IPAddress.Broadcast}'");
    }
    
    private async Task KeepAddressRequest(DhcpMessage message)
    {
        var addressRequestData = message.Options[DhcpMessage.DhcpOptionCode.RequestedIpAddress];
        var addressRequest = new IPAddress(addressRequestData);
       // Log.Debug($"[REQUEST] {message.ClientHardwareAddress} has requested to keep it's IP Address '{addressRequest}'");

        // if (addressRequest.IsInSameSubnet(Configuration.StartIpAddress, Configuration.SubnetMask) == false)
        // {
        //    // Log.Debug($"[REQUEST] {message.ClientHardwareAddress} request for '{addressRequest}' has been DENIED due to subnet mismatch");
        //     await this.SendNak(message, addressRequest).ConfigureAwait(false);
        //     return;
        // }

        var keepReservationResponse = true; //await LeaseManager.KeepLeaseRequest(addressRequest, message.ClientHardwareAddress, message.HostName);
        if (keepReservationResponse)
        {
            await SendAck(message, addressRequest);
           // Log.Debug($"[REQUEST] {message.ClientHardwareAddress} has been approved!");
            return;
        }

        await this.SendNak(message, addressRequest);
      // Log.Debug($"[REQUEST] {message.ClientHardwareAddress} has been DENIED.");
    }
    
    private async Task SendNak(DhcpMessage message, IPAddress requestedAddress)
    {
        //Log.Debug("[NAK] Creating the negative acknowledgement");

        message.OperationCode = DhcpMessage.DhcpOperation.BootReply;
        message.YourIPAddress = requestedAddress;
        message.ServerIPAddress = DhcpInterfaceAddress;

        var optionBuilder = new DhcpOptionBuilder();
        optionBuilder.AddOption(DhcpMessage.DhcpOptionCode.DhcpMessageType,DhcpMessage.DhcpMessageType.Nak);

        var packet = message.ToPacket(optionBuilder.GetBytes());

        await SendReply(packet);

        //Log.Debug($"[NAK] IP Address '{requestedAddress}' was sent over '{IPAddress.Broadcast}'");
    }
    
    public async Task Unknown(DhcpMessage message)
    {
        
    }
    
}
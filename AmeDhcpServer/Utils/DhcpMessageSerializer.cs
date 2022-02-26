using System.Net;
using System.Net.NetworkInformation;
using AmeDhcpServer.Application.DhcpModel;
using AmeDhcpServer.Core;

namespace AmeDhcpServer.Utils
{
    public static class DhcpMessageSerializer
    {
        public static DhcpMessage ToMessage(this DhcpPacket packet)
        {
            DhcpOptionParser parser = new DhcpOptionParser();
            DhcpMessage result = new DhcpMessage
            {
                OperationCode = (DhcpMessage.DhcpOperation)packet.Op,
                Hardware = (DhcpMessage.HardwareType)packet.HType,
                HardwareAddressLength = packet.HLen,
                Hops = packet.Hops,
                TransactionId = BitConverter.ToInt32(packet.XID.Reverse().ToArray(), 0),
                SecondsElapsed = BitConverter.ToUInt16(packet.Secs, 0),
                Flags = BitConverter.ToUInt16(packet.Flags, 0),
                ClientIPAddress = new IPAddress(packet.CIAddr),
                YourIPAddress = new IPAddress(packet.YIAddr),
                ServerIPAddress = new IPAddress(packet.SIAddr),
                GatewayIPAddress = new IPAddress(packet.GIAddr),
                ClientHardwareAddress = new PhysicalAddress(packet.CHAddr.Take(packet.HLen).ToArray()),
                File = packet.File,
                Cookie = packet.Cookie,
                Options = parser.GetOptions(packet.Options)
            };

            return result;
        }

        public static DhcpPacket ToPacket(this DhcpMessage message, byte[] options)
        {
            DhcpPacket packet = new DhcpPacket
            {
                Op = (byte) message.OperationCode,
                HType = (byte) message.Hardware,
                Hops = (byte) message.Hops,
                XID = BitConverter.GetBytes(message.TransactionId).Reverse().ToArray(),
                Secs = BitConverter.GetBytes(message.SecondsElapsed),
                Flags = BitConverter.GetBytes(message.Flags),
                CIAddr = message.ClientIPAddress.GetAddressBytes(),
                YIAddr = message.YourIPAddress.GetAddressBytes(),
                SIAddr = message.ServerIPAddress.GetAddressBytes(),
                GIAddr = message.GatewayIPAddress.GetAddressBytes(),
                Options = options
            };

            packet = SetClientHardwareAddressFields(packet, message);
            return packet;
        }
        
        public static DhcpPacket ToPacket(this DhcpMessage message)
        {
            var optionBuilder = new DhcpOptionBuilder();
            foreach (var option in message.Options)
            {
                optionBuilder.AddOption(option.Key, option.Value);
            }
            
            DhcpPacket packet = new DhcpPacket
            {
                Op = (byte) message.OperationCode,
                HType = (byte) message.Hardware,
                Hops = (byte) message.Hops,
                XID = BitConverter.GetBytes(message.TransactionId).Reverse().ToArray(),
                Secs = BitConverter.GetBytes(message.SecondsElapsed),
                Flags = BitConverter.GetBytes(message.Flags),
                CIAddr = message.ClientIPAddress.GetAddressBytes(),
                YIAddr = message.YourIPAddress.GetAddressBytes(),
                SIAddr = message.ServerIPAddress.GetAddressBytes(),
                GIAddr = message.GatewayIPAddress.GetAddressBytes(),
                Options = optionBuilder.GetBytes()
            };

            packet = SetClientHardwareAddressFields(packet, message);
            return packet;
        }

        private static DhcpPacket SetClientHardwareAddressFields(DhcpPacket packet, DhcpMessage message)
        {
            var addressBytes = message.ClientHardwareAddress.GetAddressBytes();
            var addressLength = addressBytes.Length;
            byte[] chAddressArray = new byte[16];

            addressBytes.CopyTo(chAddressArray, 0);

            packet.CHAddr = chAddressArray;
            packet.HLen = (byte)addressLength;

            return packet;
        }
    }
}

using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using AmeServer.Core;

namespace AmeServer.Utils
{
    public static class DhcpMessageSerializer
    {
        private static readonly byte[] DhcpOptionsMagicNumber = {99,130,83,99};
        public static DhcpMessage ToDhcpMessage(this byte[] data)
        {
            using var reader = new BinaryReader( new MemoryStream(data), Encoding.Default, false);
            DhcpMessage result = new DhcpMessage
            {
                OperationCode = (DhcpMessage.DhcpOperation)reader.ReadByte(),
                Hardware = reader.ReadByte(),
                HardwareAddressLength = reader.ReadByte(),
                Hops = reader.ReadByte(),
                TransactionId = reader.ReadInt32(),
                SecondsElapsed = reader.ReadUInt16(),
                Flags = reader.ReadUInt16(),
                ClientIPAddress = new IPAddress(reader.ReadBytes(4)),
                YourIPAddress = new IPAddress(reader.ReadBytes(4)),
                ServerIPAddress = new IPAddress(reader.ReadBytes(4)),
                GatewayIPAddress = new IPAddress(reader.ReadBytes(4))
            };
            result.ClientHardwareAddress =
                new PhysicalAddress(reader.ReadBytes(16).Take(result.HardwareAddressLength).ToArray());
            result.ServerName = reader.ReadBytes(64);
            result.File = reader.ReadBytes(128);
            result.Cookie = reader.ReadBytes(4);
            
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var opCode = (DhcpMessage.DhcpOptionCode)reader.ReadByte();
                if (opCode == DhcpMessage.DhcpOptionCode.End) break;
                var length = reader.ReadByte();
                result.Options[opCode] = reader.ReadBytes(length);
            }
            return result;
        }
        
        public static byte[] ToBytes(this DhcpMessage packet)
        {
            using var mem = new MemoryStream(512);
            using var writer = new BinaryWriter(mem);
            writer.Write((byte)packet.OperationCode);
            writer.Write(packet.Hardware);
            writer.Write(packet.HardwareAddressLength);
            writer.Write(packet.Hops);
            writer.Write(packet.TransactionId);
            writer.Write(packet.SecondsElapsed);
            writer.Write(packet.Flags);

            writer.Write(packet.ClientIPAddress.GetAddressBytes());
            writer.Write(packet.YourIPAddress.GetAddressBytes());
            writer.Write(packet.ServerIPAddress.GetAddressBytes());
            writer.Write(packet.GatewayIPAddress.GetAddressBytes());
            var clientHardwareAddress = packet.ClientHardwareAddress.GetAddressBytes();
            writer.Write(clientHardwareAddress);
            writer.Write(new byte[16 - clientHardwareAddress.Length]);

            byte[] snameBytes = new byte[64];
            writer.Write(snameBytes);

            byte[] fileBytes = new byte[128];
            writer.Write(fileBytes);
            writer.Write(DhcpOptionsMagicNumber);

            foreach (var option in packet.Options)
            {
                writer.Write((byte)option.Key);
                writer.Write((byte)option.Value.Length);
                writer.Write(option.Value);
            }
            writer.Write((byte)DhcpMessage.DhcpOptionCode.End);
            writer.Flush();
            return mem.ToArray();
        }
    }
}

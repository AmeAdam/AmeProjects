using System.Net;
using AmeDhcpServer.Core;

namespace AmeDhcpServer.Application.DhcpModel
{
    public class DhcpOptionBuilder
    {
        private readonly List<byte> _bytes;

        public DhcpOptionBuilder()
        {
            _bytes = new List<byte>();
        }
        public byte[] GetBytes()
        {
            var byteArray = _bytes.ToArray();
            byteArray = AppendEndByte(byteArray);
            return byteArray;
        }
        public void AddOption(DhcpMessage.DhcpOptionCode opCode, int data, bool isReversed)
        {
            var bytes = BitConverter.GetBytes(data);

            AddOption(opCode, isReversed ? bytes.Reverse().ToArray() : bytes);
        }
        public void AddOption(DhcpMessage.DhcpOptionCode opCode, DhcpMessage.DhcpMessageType messageType)
        {
            AddOption(opCode, (byte)messageType);
        }
        public void AddOption(DhcpMessage.DhcpOptionCode opCode, IPAddress data)
        {
            AddOption(opCode, data.GetAddressBytes());
        }
        public void AddOption(DhcpMessage.DhcpOptionCode opCode, byte data)
        {
            AddOption(opCode, new[] { data });
        }
        public void AddOption(DhcpMessage.DhcpOptionCode opCode, byte[] data)
        {
            _bytes.Add((byte)opCode);
            _bytes.Add((byte)data.Length);
            _bytes.AddRange(data);
        }

        private byte[] AppendEndByte(byte[] byteArray)
        {
            var newArray = new byte[byteArray.Length + 1];
            byteArray.CopyTo(newArray, 0);
            newArray[newArray.Length-1] = (byte)DhcpMessage.DhcpOptionCode.End;
            return newArray;
        }
    }
}

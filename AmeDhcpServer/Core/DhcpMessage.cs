using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace AmeDhcpServer.Core
{
    public class DhcpMessage
    {
        public DhcpOperation OperationCode { get; set; }
        public HardwareType Hardware{ get; set; }
        public ushort HardwareAddressLength { get; set; }
        public ushort Hops { get; set; }
        public int TransactionId { get; set; }
        public ushort SecondsElapsed { get; set; }
        public ushort Flags { get; set; }
        public IPAddress ClientIPAddress { get; set; }
        public IPAddress YourIPAddress { get; set; }
        public IPAddress ServerIPAddress { get; set; }
        public IPAddress GatewayIPAddress { get; set; }
        public PhysicalAddress ClientHardwareAddress { get; set; }
        public byte[] File { get; set; }
        public byte[] Cookie { get; set; }
        public Dictionary<DhcpOptionCode, byte[]> Options { get; set; }

        public DhcpMessageType MessageType => Options.TryGetValue(DhcpOptionCode.DhcpMessageType, out var type) ? (DhcpMessageType)type[0] : DhcpMessageType.Unknown;
        public string? HostName => Options.TryGetValue(DhcpOptionCode.Hostname, out var hostname) ? Encoding.Default.GetString(hostname) : null;

        public DhcpMessage CreateReplay(IPAddress yourAddress, IPAddress serverAddress)
        {
            var cloned = (DhcpMessage)MemberwiseClone();
            cloned.Options = new Dictionary<DhcpOptionCode, byte[]>();
            cloned.OperationCode = DhcpOperation.BootReply;
            cloned.YourIPAddress = yourAddress;
            cloned.ServerIPAddress = serverAddress;
            return cloned;
        }
        
        public void AddOption(DhcpOptionCode opCode, int data, bool isReversed)
        {
            var bytes = BitConverter.GetBytes(data);

            AddOption(opCode, isReversed ? bytes.Reverse().ToArray() : bytes);
        }
        public void AddOption(DhcpOptionCode opCode, DhcpMessageType messageType)
        {
            AddOption(opCode, (byte)messageType);
        }
        public void AddOption(DhcpOptionCode opCode, IPAddress data)
        {
            AddOption(opCode, data.GetAddressBytes());
        }
        public void AddOption(DhcpOptionCode opCode, byte data)
        {
            AddOption(opCode, new[] { data });
        }
        public void AddOption(DhcpOptionCode opCode, byte[] data)
        {
            Options[opCode] = data;
        }

        public enum DhcpMessageType
        {
            Unknown = 0x00,
            Discover,
            Offer,
            Request,
            Decline,
            Ack,
            Nak,
            Release,
            Inform,
            ForceRenew,
            LeaseQuery,
            LeaseUnassigned,
            LeaseUnknown,
            LeaseActive
        }
        
        public enum DhcpOperation :byte
        {
            BootRequest = 0x01,
            BootReply
        }
        
        public enum DhcpOptionCode : byte
        {
            Pad = 0x00,
            SubnetMask = 0x01,
            TimeOffset = 0x02,
            Router = 0x03,
            TimeServer = 0x04,
            NameServer = 0x05,
            DomainNameServer = 0x06,
            LogServer = 0x07,
            CookieServer = 0x08,
            LprServer = 0x09,
            ImpressServer = 0x0A,
            ResourceLocationServer = 0x0B,
            Hostname = 0x0C,
            BootFileSize = 0x0D,
            MeritDumpFile = 0x0E,
            DomainNameSuffix = 0x0F,
            NetBIOSOverTCPIPNodeType = 0x2E,
            RequestedIpAddress = 0x32,
            AddressTime = 0x33,
            DhcpMessageType = 0x35,
            DhcpAddress = 0x36,
            ParameterList = 0x37,
            DhcpMessage = 0x38,
            DhcpMaxMessageSize = 0x39,
            ClassId = 0x3C,
            ClientId = 0x3D,
            AutoConfig = 0x74,
            End = 0xFF
        }
        
        public enum HardwareType
        {
            Ethernet = 0x01,
            ExperimentalEthernet,
            AmateurRadio,
            ProteonTokenRing,
            Chaos,
            IEEE802Networks,
            ArcNet,
            Hyperchannel,
            Lanstar
        }


    }
}

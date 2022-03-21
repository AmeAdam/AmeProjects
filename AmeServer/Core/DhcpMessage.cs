using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace AmeServer.Core
{
    public class DhcpMessage
    {
        public DhcpOperation OperationCode { get; set; }
        public byte Hardware{ get; set; }
        public byte HardwareAddressLength { get; set; }
        public byte Hops { get; set; }
        public int TransactionId { get; set; }
        public ushort SecondsElapsed { get; set; }
        public ushort Flags { get; set; }
        public IPAddress ClientIPAddress { get; init; } = IPAddress.None;
        public IPAddress YourIPAddress { get; set; }= IPAddress.None;
        public IPAddress ServerIPAddress { get; set; }= IPAddress.None;
        public IPAddress GatewayIPAddress { get; init; }= IPAddress.None;
        public PhysicalAddress ClientHardwareAddress { get; set; } = PhysicalAddress.None;
        public byte[] ServerName { get; set; } = Array.Empty<byte>();
        public byte[] File { get; set; } = Array.Empty<byte>();
        public byte[] Cookie { get; set; } = Array.Empty<byte>();
        public Dictionary<DhcpOptionCode, byte[]> Options { get; set; } = new();

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
            NetBiosOverTcpIpNodeType = 0x2E,
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
    }
}

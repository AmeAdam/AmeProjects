﻿namespace AmeDhcpServer.Core
{
    public class DhcpPacket
    {
        /// <summary>
        /// Operation Code (1 byte)
        /// 1: BootRequest
        /// 2: Boot Reply
        /// </summary>
        public byte Op { get; set; }

        /// <summary>
        /// Hardware Type (1 byte)
        /// </summary>
        public byte HType { get; set; }

        /// <summary>
        /// Hardware Address Length (1 byte)
        /// Length of the MAC Address
        /// </summary>
        public byte HLen { get; set; }

        /// <summary>
        /// Hops (1 byte)
        /// Set to 0 by a client before transmitting a request
        /// and used by relay agents to control the forwarding of BOOTP and/or DHCP messages.
        /// </summary>
        public byte Hops { get; set; }

        /// <summary>
        /// Transaction Identifier (4 bytes)
        /// A 32-bit identification field generated by the client, 
        /// to allow it to match up the request with replies received from DHCP servers.
        /// </summary>
        public byte[] XID { get; set; }

        /// <summary>
        /// Seconds (2 bytes)
        /// Number of seconds elapsed since a client began an attempt to acquire or renew a lease.
        /// </summary>
        public byte[] Secs { get; set; }

        /// <summary>
        /// Flags (2 bytes)
        /// </summary>
        public byte[] Flags { get; set; }

        /// <summary>
        /// Client IP Address (4 bytes)
        /// The client puts its own current IP address in this field 
        /// if and only if it has a valid IP address while in the 
        /// BOUND, RENEWING or REBINDING states; otherwise, it sets the field to 0.
        /// </summary>
        public byte[] CIAddr { get; set; }

        /// <summary>
        /// "Your" IP Address (4 bytes)
        /// The IP Address that the server is assigning to the client
        /// </summary>
        public byte[] YIAddr { get; set; }

        /// <summary>
        /// Server IP Address (4 bytes)
        /// The address of the server that the client should use for the next step in the bootstrap process
        /// </summary>
        public byte[] SIAddr { get; set; }

        /// <summary>
        /// Gateway IP Address (4 bytes)
        /// This field is not used by clients,
        /// and does not represent the server giving the client the address of a default router
        /// </summary>
        public byte[] GIAddr { get; set; }

        /// <summary>
        /// Client Hardware Address (16 bytes)
        /// The hardware (layer two) address of the client, which is used for identification and communication.
        /// Generally the first 6 bytes are the MAC and the rest are padding
        /// </summary>
        public byte[] CHAddr { get; set; }

        /// <summary>
        /// Server Name (64 bytes)
        /// The server sending a DHCPOFFER or DHCPACK message may optionally put its name in this field
        /// </summary>
        public byte[] SName { get; set; }

        /// <summary>
        /// Boot Filename (128 bytes)
        /// Used by a server in a DHCPOFFER to fully specify a boot file directory path and filename.
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// Magic Cookie (4 bytes)
        /// Used to determine the difference between a BOOTP and DHCP packet
        /// 99.130.83.99
        /// </summary>
        public byte[] Cookie { get; set; }

        /// <summary>
        /// Options (variable)
        /// The rest of the data set
        /// </summary>
        public byte[] Options { get; set; }
    }
}

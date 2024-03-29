﻿using AmeServer.Core.Entities;

namespace AmeServer.Core.Events;

public class DhcpRequestRejectedEvent : IDomainEvent
{
    public NetworkDevice NetworkDevice { get; }
    public DhcpMessage RequestMessage { get; }

    public DhcpRequestRejectedEvent(NetworkDevice networkDevice, DhcpMessage requestMessage)
    {
        NetworkDevice = networkDevice;
        RequestMessage = requestMessage;
    }
}
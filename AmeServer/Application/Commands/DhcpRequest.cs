using System.Net;
using AmeServer.Core;
using AmeServer.Core.Entities;
using AmeServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTools;

namespace AmeServer.Application.Commands;

public static class DhcpRequest
{
    public record Command(DhcpMessage Message) : IRequest<Unit>;

    // ReSharper disable once UnusedType.Global
    public class Handler :IRequestHandler<Command, Unit>
    {
        private readonly ApplicationContext context;
        private readonly IMediator mediator;
        private readonly ILogger<Handler> logger;

        public Handler(ApplicationContext context, IMediator mediator, ILogger<Handler> logger)
        {
            this.context = context;
            this.mediator = mediator;
            this.logger = logger;
        }

        private async Task<IPAddress> GetAvailableAddress(NetworkConfiguration config, CancellationToken cancellationToken)
        {
            var allAddresses = 
                await context.NetworkDevices.Select(nd => nd.ClientAddress).ToListAsync(cancellationToken);
            
            IPAddressRange range = new IPAddressRange(config.PoolMin, config.PoolMax);
            var freeAddress = range.AsEnumerable().FirstOrDefault(address => !allAddresses.Contains(address));
            return freeAddress ?? throw new ApplicationException("Unable to assign new address");
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var device = await context.NetworkDevices
                .Include(d => d.PreferredConfiguration)
                .FirstOrDefaultAsync(nd => nd.Id.Equals(request.Message.ClientHardwareAddress), cancellationToken);

            if (device == null)
            {
                var config = context.NetworkConfigurations.OrderByDescending(c => c.Priority).First();
                var nexAddress = await GetAvailableAddress(config, cancellationToken);
                
                device = new NetworkDevice(request.Message.ClientHardwareAddress, request.Message.HostName ?? "",
                    nexAddress, false, config);
                await context.NetworkDevices.AddAsync(device, cancellationToken);
            }
            
            switch (request.Message.MessageType)
            {
                case DhcpMessage.DhcpMessageType.Discover:
                    logger.LogInformation("Discover {Mac} {Client}", request.Message.ClientHardwareAddress, request.Message.HostName);
                    device.Discover(request.Message);
                    break;
                case DhcpMessage.DhcpMessageType.Request:
                    logger.LogInformation("Request {Mac} {Client} {ClientIp}", request.Message.ClientHardwareAddress, request.Message.HostName, request.Message.ClientIPAddress);
                    device.Request(request.Message);
                    break;
            }
            
            foreach (var deviceEvent in device.Events)
            {
                await mediator.Publish(deviceEvent, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
using AmeDhcpServer.Application.Exceptions;
using AmeDhcpServer.Core;
using AmeDhcpServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AmeDhcpServer.Application.Commands;

public static class DhcpRequest
{
    public record Command(DhcpMessage Message) : IRequest<Unit>;

    public class Handler :IRequestHandler<Command, Unit>
    {
        private ApplicationContext context;
        private readonly IMediator mediator;
        private readonly ILogger<Handler> logger;

        public Handler(ApplicationContext context, IMediator mediator, ILogger<Handler> logger)
        {
            this.context = context;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var device = await context.NetworkDevices.FirstOrDefaultAsync(
                    nd => nd.Id.Equals(request.Message.ClientHardwareAddress), cancellationToken);

            if (device == null)
            {
                logger.LogError("Device not registered {Mac} {Client}", request.Message.ClientHardwareAddress, request.Message.HostName);
                throw new DeviceNotRegisteredException($"Device mac: {request.Message.ClientHardwareAddress}, hostname: {request.Message.HostName} not registered");
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
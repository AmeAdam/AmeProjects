namespace AmeDhcpServer.Application.Exceptions;

public class DeviceNotRegisteredException : ApplicationException
{
    public DeviceNotRegisteredException(string? message) : base(message)
    {
    }
}
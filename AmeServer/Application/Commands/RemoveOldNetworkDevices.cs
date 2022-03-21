using AmeServer.Infrastructure;
using Quartz;

namespace AmeServer.Application.Commands;

// ReSharper disable once ClassNeverInstantiated.Global
public class RemoveOldNetworkDevices : IJob
{
    private readonly ApplicationContext context;

    public RemoveOldNetworkDevices(ApplicationContext context)
    {
        this.context = context;
    }

    public async Task Execute(IJobExecutionContext quartzContext)
    {
        var oldEntries = context.NetworkDevices.Where(nd => !nd.Permanent && nd.UpdateTime < DateTime.Now.AddDays(7));
        context.NetworkDevices.RemoveRange(oldEntries);
        await context.SaveChangesAsync();
    }
}
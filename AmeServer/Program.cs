using System.Reflection;
using AmeServer.Application;
using AmeServer.Application.Commands;
using AmeServer.Application.Services;
using AmeServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath =  AppContext.BaseDirectory,
    WebRootPath = "wwwroot",
    Args = args
});

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
builder.Host.UseWindowsService();

// Add services to the container.
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ServiceWorker>();
builder.Services.AddSingleton<IUdpSender, UdpService>();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<RemoveOldNetworkDevices>(trigger => trigger
        .WithIdentity("cleanup")
        .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10)))
        .WithSimpleSchedule(schedule =>
        {
            schedule.RepeatForever();
            schedule.WithIntervalInHours(24);
        })
    );
});
builder.Services.AddQuartzServer(config => { config.WaitForJobsToComplete = true; });
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();

    // if (!await context.NetworkConfigurations.AnyAsync())
    // {
    //     await context.NetworkConfigurations.AddAsync(new NetworkConfiguration("T-Mobile",
    //         IPAddress.Parse("192.168.8.1"), IPAddress.Parse("255.255.255.0"), IPAddress.Parse("192.168.8.1"),
    //         IPAddress.Parse("8.8.8.8"), IPAddress.Parse("192.168.8.3")));
    //     await context.NetworkConfigurations.AddAsync(new NetworkConfiguration("Nju",
    //         IPAddress.Parse("192.168.8.2"), IPAddress.Parse("255.255.255.0"), IPAddress.Parse("192.168.8.2"),
    //         IPAddress.Parse("8.8.8.8"), IPAddress.Parse("192.168.8.3")));
    //     await scope.ServiceProvider.GetRequiredService<ImportService>().ImportFromIni(@"C:\dhcpsrv2.5.2\dhcpsrv.ini");
    //     await context.SaveChangesAsync();
    // }
}

app.Run();
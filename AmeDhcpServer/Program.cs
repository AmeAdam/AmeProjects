using System.Reflection;
using AmeDhcpServer.Application;
using AmeDhcpServer.Application.Services;
using AmeDhcpServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddSingleton<DhcpService>();
builder.Services.AddScoped<ImportService>();
builder.Services.AddSingleton<IUdpSender, UdpService>();

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

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
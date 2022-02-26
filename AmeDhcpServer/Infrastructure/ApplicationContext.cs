using AmeDhcpServer.Core;
using AmeDhcpServer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmeDhcpServer.Infrastructure;

//dotnet ef migrations add InitialCreate
public class ApplicationContext : DbContext
{
    public DbSet<NetworkDevice> NetworkDevices { get; set; }
    public DbSet<NetworkConfiguration> NetworkConfigurations { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NetworkDevice>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(256);
            builder.Property(e => e.Permanent);
            builder.Property(e => e.LeaseTimeSeconds);
            builder.Property(e => e.UpdateTime);
            builder.Property(e => e.PreferredNetworkConfiguration);
            builder.Property(e => e.ClientAddress);
            builder.Property(e => e.State);
            builder.Property(e => e.PresentedName);
        });

        modelBuilder.Entity<NetworkConfiguration>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(256);
            builder.Property(e => e.Gateway1);
            builder.Property(e => e.Gateway2);
            builder.Property(e => e.Dns1);
            builder.Property(e => e.Dns2);
            builder.Property(e => e.SubnetMask);
            builder.Property(e => e.Dhcp);
        });
    }
}
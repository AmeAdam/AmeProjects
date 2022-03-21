using System.Net;
using AmeServer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmeServer.Infrastructure;

//dotnet ef migrations add InitialCreate
public class ApplicationContext : DbContext
{
    public DbSet<NetworkDevice> NetworkDevices => Set<NetworkDevice>();
    public DbSet<NetworkConfiguration> NetworkConfigurations => Set<NetworkConfiguration>();

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
            builder.Property(e => e.ClientAddress);
            builder.Property(e => e.State);
            builder.Property(e => e.PresentedName);
            builder.HasOne(e => e.PreferredConfiguration).WithMany();
        });

        modelBuilder.Entity<NetworkConfiguration>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(256);
            builder.Property(e => e.Gateways).HasConversion(
                a => IpAddressesToString(a), 
                txt => IpAddressesFromString(txt));
            builder.Property(e => e.Dns)
                .HasConversion(
                    a => IpAddressesToString(a), 
                    txt => IpAddressesFromString(txt));
            builder.Property(e => e.SubnetMask);
            builder.Property(e => e.Dhcp);
            builder.Property(e => e.PoolMin);
            builder.Property(e => e.PoolMax);
            builder.Property(e => e.Priority);
        });
    }

    private static string IpAddressesToString(IPAddress[] addresses)
    {
        return string.Join(",", addresses.Select(a => a.ToString()));
    }

    private static IPAddress[] IpAddressesFromString(string str)
    {
        return str
            .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(IPAddress.Parse)
            .ToArray();
    }
}
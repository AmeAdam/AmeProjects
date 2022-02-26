using System.Net;
using System.Net.NetworkInformation;
using AmeDhcpServer.Core.Entities;
using AmeDhcpServer.Infrastructure;
using IniParser;
using IniParser.Model;

namespace AmeDhcpServer.Application;

public class ImportService
{
    private ApplicationContext context;

    public ImportService(ApplicationContext context)
    {
        this.context = context;
    }

    public async Task ImportFromIni(string filePath)
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filePath);
        foreach (var section in data.Sections)
        {
            if (PhysicalAddress.TryParse(section.SectionName, out var mac))
            {
                var ip = section.Keys["IPADDR"];
                var hostName = section.Keys["Hostname"];
                var isPermanent = string.IsNullOrEmpty(section.Keys["LeaseEnd"]);
                var preferedNetwork = section.Keys["ROUTER_0"] == "192.168.8.2" ? "Nju" : "T-Mobile";

                if (!string.IsNullOrEmpty(hostName) && IPAddress.TryParse(ip, out var ipAddress))
                {
                    await context.NetworkDevices.AddAsync(new NetworkDevice(mac, hostName, ipAddress, isPermanent, preferedNetwork));
                }
            }
        }
        
    }
}
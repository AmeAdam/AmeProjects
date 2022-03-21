using System;
using System.Net;
using NUnit.Framework;
using System.Linq;
using NetTools;

namespace AmeTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var minIp = new IPAddress(new byte[]{ 192, 168, 8, 100 });
        var maxIp = new IPAddress(new byte[]{ 192, 168, 8, 200 });

        IPAddressRange range = new IPAddressRange(minIp, maxIp);

        foreach (var ipAddress in range.AsEnumerable())
        {

            Console.WriteLine(ipAddress);
            
        }
        

        Assert.Pass();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;

namespace GateWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MaxConcurrentConnections = 100;
                        // Set properties and call methods on options
                        //serverOptions.Listen(IPAddress.Loopback , 5001);
                        //serverOptions.ListenAnyIP(1883, l => l.UseMqtt()); // MQTT pipeline
                        //serverOptions.ListenAnyIP(5001); // Default HTTP pipeline
                    })
                    .UseStartup<Startup>();
                });
    }
}

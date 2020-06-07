using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace GateWay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            #region MQTT setup
            string hostIp = Configuration["MqttOption:HostIp"];
            int hostPort = int.Parse(Configuration["MqttOption:HostPort"]);
            int timeout = int.Parse(Configuration["MqttOption:Timeout"]);
            string username = Configuration["MqttOption:UserName"];
            string password = Configuration["MqttOption:Password"];
            //MQTT Builder
            var optionBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse(hostIp))
                .WithDefaultEndpointPort(hostPort)
                .WithDefaultCommunicationTimeout(TimeSpan.FromMilliseconds(timeout))
                .WithConnectionValidator(validator =>
                {
                    if (validator.Username != username || validator.Password != password)
                    {
                        validator.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    validator.ReasonCode = MqttConnectReasonCode.Success;
                });
            var option = optionBuilder.Build();
            services
               .AddHostedMqttServer(option)
               .AddMqttConnectionHandler()
               .AddConnections();
            #endregion
            services.AddHttpClient();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMqtt("/mqtt");
            });

            app.UseMqttServer(server =>
            {
                // Todo: Do something with the server

            });
        }
    }
}

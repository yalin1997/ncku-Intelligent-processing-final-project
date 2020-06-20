using CloudServer.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IGatewayControl _gatewayControl;
        public TimedHostedService(IHttpClientFactory httpClient , IGatewayControl gateWayControl)
        {
            _clientFactory = httpClient;
            _gatewayControl = gateWayControl;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(300));

            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            foreach(GatewayModel item in _gatewayControl.getGateWayList())
            {
                if (!CheckHealthyGateWay(item.gateWayUri+"/hc"))
                {
                    _gatewayControl.removeGateWay(item);
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
        private bool CheckHealthyGateWay(string url)
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            // 發出 get 並取得結果
            HttpResponseMessage response = cloudHttpSender.GetAsync(url).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (cloudResponse == "Healthy")
            {
                return true;
            }
            return false;
        }
    }
}

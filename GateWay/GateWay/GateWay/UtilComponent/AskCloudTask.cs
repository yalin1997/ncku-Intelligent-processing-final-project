using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GateWay.UtilComponent
{
    public class AskCloud : IHostedService, IDisposable
    {
        private readonly ILogger<AskCloud> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IAlarmControl _alarmControl;
        private Timer _timer;

        public AskCloud(IHttpClientFactory clientFactory, ILogger<AskCloud> logger , IConfiguration configuration , IAlarmControl alarmControl)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
            _alarmControl = alarmControl;
        }
        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("問雲端服務啟動");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogDebug("傳送問問包");
            SendAskToCloudAsync().GetAwaiter();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> SendAskToCloudAsync()
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            var postData = new CloudServer.Models.GateWayModel() { gateWayId = _configuration["GateWayId"], longitude = Convert.ToDouble(_configuration["longitude"]), latitude = Convert.ToDouble(_configuration["latitude"]) };
            // 將 data 轉為 json
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            var contentPost = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            try
            {
                var response = await cloudHttpSender.PostAsync(_configuration["CloudUri"] + "/api/CloudService/gateWayRegister", contentPost);
                string cloudResponse = await response.Content.ReadAsStringAsync();
                var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.cloudResponseModel>(cloudResponse);
                if (responseModel != null && responseModel.content.ToString() == "True")
                {
                    _logger.LogDebug("燒起來了");
                    _alarmControl.setAlarm();
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return false;
        }
    }
}

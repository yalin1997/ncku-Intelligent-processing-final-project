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
    public class RegisterTask : IHostedService, IDisposable
    {
        private readonly ILogger<RegisterTask> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private Timer _timer;

        public RegisterTask(IHttpClientFactory clientFactory, ILogger<RegisterTask> logger , IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("註冊服務啟動");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return SendRegisterToCloudAsync();
        }

        private void DoWork(object state)
        {
            _logger.LogDebug("傳送心跳包");
            SendRegisterToCloudAsync().GetAwaiter();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> SendRegisterToCloudAsync()
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            var postData = new CloudServer.Models.GateWayModel() { gateWayId = _configuration["GateWayId"], longitude = Convert.ToDouble(_configuration["longitude"]), latitude = Convert.ToDouble(_configuration["latitude"]) };
            // 將 data 轉為 json
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            var contentPost = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            try
            {
                var response = await cloudHttpSender.PostAsync(_configuration["CloudUri"] + "/api/CloudService/gateWayRegister", contentPost);
                string cloudResponse = await response.Content.ReadAsStringAsync();
                var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.cloudResponseModel>(cloudResponse);
                if (responseModel != null && responseModel.content == "true")
                {
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

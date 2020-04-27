using SmsQueueSenderService.Infrastructure.Interfaces;
using SmsQueueSenderService.Infrastructure.IoC;
using BusinessLogic.Dtos.SmsSenderService;
using Framework;
using Framework.Text;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Topshelf;

namespace SmsQueueSenderService.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private HttpClient _client;

        private dynamic _appSettings;
        private static IProcessorManager _processorManager;

        private CancellationTokenSource cancellationTokenSource;
        private Task mainTask;

        public SmsSenderService(IAppSettingsWrapper appSettingsWrapper, IProcessorManager processorManager, HttpClient client)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));

            _appSettings = appSettingsWrapper;
            _client = client;

            _processorManager = processorManager;
        }

        public bool Start(HostControl host)
        {

            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            this.mainTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    BootstrapService();

                    LogWriter.LogEntry("ATSS sending queue: " + _appSettings.SmsSenderQueueName, LoggerType.Log4Net);
                    LogWriter.LogEntry("ATSS started", LoggerType.Log4Net);

                    _processorManager.Start(cancellationToken);
                }
                catch (Exception)
                {
                    host.Stop();
                }
            }, cancellationToken);
            return true;
        }

        public bool Stop(HostControl host)
        {
            try
            {
                this.cancellationTokenSource.Cancel();
                _processorManager.Stop(cancellationTokenSource.Token);
                this.mainTask.Wait();
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "Error on shutdown", LoggerType.Log4Net);
                return false;
            }
            finally
            {
                LogWriter.LogEntry("ATSS stopped");
                _client.Dispose();
                IoCBootstrapper.Instance.Container.Dispose();
            }
        }

        private void BootstrapService()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", _appSettings.ApiAuthKey);

            HttpResponseMessage response = (HttpResponseMessage)_client.GetAsync(_appSettings.AtssSettingsEndpoint).Result;
            response.EnsureSuccessStatusCode();

            var dto = response.Content.ReadAsAsync<SmsSendersSettingsDTO>().Result;
            if (dto == null)
            {
                throw new Exception("Didn't get an answer from the api");
            }

            dto.AtssDeliveryEndpoint = _appSettings.AtssDeliveryEndpoint;
            dto.ApiAuthKey = _appSettings.ApiAuthKey;

            Program.Settings = dto;

            LogWriter.LogEntry("ATSS settings retrieve succesfuly: " + Serializer.Serialize(dto.Sms019ApiUrl), LoggerType.Log4Net);
        }
    }
}

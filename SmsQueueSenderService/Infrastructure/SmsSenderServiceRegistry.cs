using SmsQueueSenderService.Infrastructure.Interfaces;
using SmsQueueSenderService.Model;
using SmsQueueSenderService.Model.Queue;
using BusinessLogic.Billing.SMS.ATSS;
using Framework.Net.Http;
using StructureMap;
using System.Net.Http;

namespace SmsQueueSenderService.Infrastructure
{
    public class SmsSenderServiceRegistry : Registry
    {
        public SmsSenderServiceRegistry()
        {
            For<IHttpClientFactory>().Use<Framework.Net.Http.HttpClientFactory>();
            ForSingletonOf<HttpClient>().Use(c => c.GetInstance<IHttpClientFactory>().CreateHttpClient(new HttpClientOptions { RequestsPerSecond = 90 }));

            ForSingletonOf<IProcessorManager>().Use<ProcessorManager>();
            ForSingletonOf<IQueueList>().Use<QueueList>();
            
            //ForSingletonOf<ISmsMessageQueueWorker>().Use<SmsMessageQueueWorker>();
            For<ISmsMessageQueueWorker>().Use<SmsMessageQueueWorker>().AlwaysUnique().Transient();
            For<ISmsMessageHandler>().Use<SmsMessageHandler>().AlwaysUnique().Transient();
            For<ISmsSendingList>().Use<SmsSendingList>().AlwaysUnique().Transient();

            For<INexmoProxy>().Use<NexmoProxy>().AlwaysUnique().Transient();
            For<ISms019Proxy>().Use<Sms019Proxy>().AlwaysUnique().Transient();

            For<IMessagePipeline>().Use<MessagePipeline>().AlwaysUnique().Transient();
        }
    }
}

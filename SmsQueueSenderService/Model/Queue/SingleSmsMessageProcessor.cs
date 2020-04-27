//using Activetrail.SmsQueueSenderService.Infrastructure;
//using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
//using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
//using StructureMap;

//namespace Activetrail.SmsQueueSenderService.Model.Queue
//{
//    public class SingleSmsMessageProcessor<TMessage> : MessageProcessor<TMessage>
//    {
//        public SingleSmsMessageProcessor(string path, ISmsServiceConfiguration config) : base(path, config) { }

//        public SingleSmsMessageProcessor(string path, int count, ISmsServiceConfiguration config) : base(path, count, config) { }

//        protected override void Process(TMessage @object)
//        {
//            SMSMessageQueueInfo po = @object as SMSMessageQueueInfo;
//            var container = Container.For<SmsSenderServiceRegistry>();
//            ISmsSenderProxy proxy = _config.Proxies[po.ProxyType];

//            if (proxy != null)
//            {
//                proxy.SendAsync(po, Program.Settings).Wait();
//            }
//        }
//    }
//}

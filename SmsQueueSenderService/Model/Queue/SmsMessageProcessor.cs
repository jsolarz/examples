//using Activetrail.SmsQueueSenderService.Infrastructure;
//using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
//using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
//using ActiveTrail.Framework;
//using ActiveTrail.Framework.Text;
//using StructureMap;
//using System;

//namespace Activetrail.SmsQueueSenderService.Model.Queue
//{
//    public class SmsMessageProcessor<TMessage> : MessageProcessor<TMessage>
//    {
//        public SmsMessageProcessor(string path, ISmsServiceConfiguration config) : base(path, config)
//        {

//        }

//        public SmsMessageProcessor(string path, int count, ISmsServiceConfiguration config) : base(path, count, config)
//        {
//        }

//        protected override void Process(TMessage @object)
//        {
//            SMSMessageQueueInfo po = @object as SMSMessageQueueInfo;

//            //single sms 
//            if (po.ProxyType == ActiveTrail.Framework.Core.TypesSmsProxyTypes.Nexmo)
//            {
//                try
//                {
//                    LogWriter.LogEntry(new LogWriterEntry("Sending nexmo message: " + ActiveTrailSerializer.Serialize(po)) { EntryType = LogWriterEntryType.Information });
//                    var container = Container.For<Infrastructure.SmsSenderServiceRegistry>();
//                    var proxy = container.GetInstance<INexmoProxy>();
//                    proxy.Settings = Program.Settings;
//                    proxy.SendAsync(po, Program.Settings).Wait();
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.LogException(ex, string.Format("ATSS Nexmo exception thrown - {0}", ex.Message));
//                    throw;
//                }
//            }
//            else //batch sms - sms019 right now
//            {
//                try
//                {
//                    LogWriter.LogEntry(new LogWriterEntry("Adding to list for 019: " + ActiveTrailSerializer.Serialize(po)) { EntryType = LogWriterEntryType.Information });
//                    SmsSendingList.Instance.Add(po);
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.LogException(ex, string.Format("ATSS add to list error: {0}", ex.Message));
//                    throw;
//                }
//            }
//        }
//    }
//}

using Activetrail.SmsQueueSenderService.Model.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveTrail.BusinessLogic;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using System.Messaging;
using System.Text.RegularExpressions;
using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;

namespace Activetrail.SmsQueueSenderService.Infrastructure
{
    class ProcessorManager : IProcessorManager
    {
        public Dictionary<SmsCampaignSendType, IMessageProcessor> StaticProcessors { get; private set; }
        public Dictionary<string, IMessageProcessor> DynamicProcessors { get; private set; }

        private string _defaultQueueName;

        public ProcessorManager(ISmsServiceConfiguration config, dynamic appSettings)
        {
            _defaultQueueName = appSettings.SmsSenderQueueName;

            StaticProcessors = new Dictionary<SmsCampaignSendType, IMessageProcessor>{
               { SmsCampaignSendType.Regular, new SmsMessageProcessor<SMSMessageQueueInfo>(appSettings.SmsSenderQueueName, 2, config)},
               { SmsCampaignSendType.TMM, new SingleSmsMessageProcessor<SMSMessageQueueInfo>(appSettings.SmsTmmSenderQueueName, 2, config) },
               { SmsCampaignSendType.Test, new SingleSmsMessageProcessor<SMSMessageQueueInfo>(appSettings.SmsTestSenderQueueName, 2, config) },
            };

            FillDynamicProcessors(config);
        }

        public void StartAllProcessors()
        {
            StaticProcessors.Values.ForEach(x => x.Open());
            DynamicProcessors.Values.ForEach(x => x.Open());
        }
        
        public void StopAllProcessors()
        {
            StaticProcessors.Values.ForEach(x => x.Close());
            DynamicProcessors.Values.ForEach(x => x.Close());
        }

        private void FillDynamicProcessors(ISmsServiceConfiguration config)
        {
            var queues= MessageQueue.GetPrivateQueuesByMachine("localhost").Where(x=> Regex.IsMatch(x.QueueName, "sms_sending_queue_[+?\\d]")).ToList();
            DynamicProcessors = new Dictionary<string, IMessageProcessor>();

            foreach (var item in queues)
            {
                var path = "." + item.Path.Substring(item.Path.ToLower().IndexOf(@"\private"));

                DynamicProcessors.Add(item.QueueName.Substring(_defaultQueueName.Length - 1), new SmsMessageProcessor<SMSMessageQueueInfo>(path, 1, config));
            }
        }
    }
}

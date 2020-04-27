using SmsQueueSenderService.Infrastructure.Interfaces;
using BusinessLogic;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Text.RegularExpressions;

namespace SmsQueueSenderService.Model.Queue
{
    public class QueueList : IQueueList
    {
        public QueueList()
        {
            var collection = new Dictionary<string, string>
            {
                { SmsCampaignSendType.Regular.ToString("F"), ConfigurationManager.AppSettings["SmsSenderQueueName"] },
                { SmsCampaignSendType.TMM.ToString("F"), ConfigurationManager.AppSettings["SmsTmmSenderQueueName"]},
                { SmsCampaignSendType.Test.ToString("F"), ConfigurationManager.AppSettings["SmsTestSenderQueueName"]}
            };

            //getting user queues
            var _defaultQueueName = ConfigurationManager.AppSettings["SmsSenderQueueName"];
            var queues = MessageQueue.GetPrivateQueuesByMachine("localhost").Where(x => Regex.IsMatch(x.QueueName, "sms_sending_queue_[+?\\d]")).ToList();

            foreach (var item in queues)
            {
                var path = "." + item.Path.Substring(item.Path.ToLower().IndexOf(@"\private"));
                var name = item.QueueName.Substring(_defaultQueueName.Length - 1);
                collection.Add("user_" + name, path);
            }

            this.Queues = collection;
        }
        public Dictionary<string, string> Queues { get; set; }

        public bool IsSendingQueue(KeyValuePair<string, string> queue)
        {
            return queue.Key.ToLowerInvariant().Equals(SmsCampaignSendType.TMM.ToString("F").ToLowerInvariant())
                || queue.Key.ToLowerInvariant().Equals(SmsCampaignSendType.Automation.ToString("F").ToLowerInvariant())
                || queue.Key.ToLowerInvariant().Equals(SmsCampaignSendType.Test.ToString("F").ToLowerInvariant());

        }

        public bool IsBatchQueue(KeyValuePair<string, string> queue)
        {
            return queue.Key.ToLowerInvariant().Equals(SmsCampaignSendType.Regular.ToString("F").ToLowerInvariant())
                || queue.Key.ToLowerInvariant().Equals(SmsCampaignSendType.Campaign.ToString("F").ToLowerInvariant())
                ||  queue.Key.ToLowerInvariant().Contains(@"user_");
        }
    }
}

using Activetrail.SmsQueueSenderService.Infrastructure;
using Activetrail.SmsQueueSenderService.Infrastructure.IoC;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using ActiveTrail.Framework;
using ActiveTrail.Framework.Core;
using ActiveTrail.Framework.Text;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace Activetrail.SmsQueueSenderService.Model
{
    public class SmsMessageSender : ISmsMessageSender
    {
        private readonly ISmsServiceConfiguration _config;

        public SmsMessageSender(ISmsServiceConfiguration config)
        {
            _config = config;
        }

        public void Process(SMSMessageQueueInfo message)
        {
            ISmsSenderProxy proxy = _config.Proxies[message.ProxyType];

            message.AddedOn = DateTime.Now;
            switch (message.PriorityQueue)
            {
                case SmsPriorityQueue.Operational:
                case SmsPriorityQueue.Test:
                    if (proxy != null)
                    {                        
                        message.Status = 2;
                        SmsSendingList.Instance.Add(message);
                        proxy.Send(message, Program.Settings);
                    }
                    break;
                case SmsPriorityQueue.General:
                case SmsPriorityQueue.Private:
                    if (message.ProxyType == TypesSmsProxyTypes.Nexmo)
                    {
                        message.Status = 2;
                        SmsSendingList.Instance.Add(message);
                        proxy.Settings = Program.Settings;
                        proxy.Send(message, Program.Settings);
                    }
                    else //batch sms - sms019 right now
                    {
                        message.Status = 1;
                        SmsSendingList.Instance.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }

        public async Task ProcessAsync(SMSMessageQueueInfo message)
        {
            ISmsSenderProxy proxy = _config.Proxies[message.ProxyType];

            message.AddedOn = DateTime.Now;
            switch (message.PriorityQueue)
            {
                case SmsPriorityQueue.Operational:
                case SmsPriorityQueue.Test:
                    if (proxy != null)
                    {
                        message.Status = 2;
                        SmsSendingList.Instance.Add(message);
                        await proxy.SendAsync(message, Program.Settings);
                    }
                    break;
                case SmsPriorityQueue.General:
                case SmsPriorityQueue.Private:
                    if (message.ProxyType == TypesSmsProxyTypes.Nexmo)
                    {
                        message.Status = 2;
                        SmsSendingList.Instance.Add(message);
                        proxy.Settings = Program.Settings;
                        await proxy.SendAsync(message, Program.Settings);
                    }
                    else //batch sms - sms019 right now
                    {
                        message.Status = 1;
                        SmsSendingList.Instance.Add(message);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

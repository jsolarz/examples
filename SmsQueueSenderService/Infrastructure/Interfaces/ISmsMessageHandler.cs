using BusinessLogic;
using BusinessLogic.Billing.SMS.ATSS;
using System;
using System.Collections.Generic;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsMessageHandler : IDisposable
    {
        bool HandleMessage(SmsCampaignSendType queueType, SMSMessageQueueInfo message);
        bool HandleMessages(List<SMSMessageQueueInfo> messages);
    }
}
using BusinessLogic.Billing.SMS.ATSS;
using System;
using System.Collections.Generic;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsMessageQueueWorker : IDisposable
    {
        List<SMSMessageQueueInfo> Dequeue(string queuePath, int count);
    }
}
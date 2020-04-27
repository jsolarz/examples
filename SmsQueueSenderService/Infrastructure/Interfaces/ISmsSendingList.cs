using SmsQueueSenderService.Model;
using BusinessLogic.Billing.SMS.ATSS;
using System;
using System.Collections.Generic;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsSendingList: IDisposable
    {
        void Add(SMSMessageQueueInfo po);
        void BulkAdd(List<SMSMessageQueueInfo> batch);
        List<SMSMessageQueueInfo> Get();
        void UpdateStatus(SmsMessageStatus status, IEnumerable<SMSMessageQueueInfo> messages);
        void UpdateStatus(SmsMessageStatus sENT, SMSMessageQueueInfo message);
    }
}
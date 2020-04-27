using System.Collections.Generic;
using Activetrail.SmsQueueSenderService.Dashboard.Models;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using ActiveTrail.Framework.Core;

namespace Activetrail.SmsQueueSenderService.Dashboard.Repositories
{
    public interface ISmsCounterRepository
    {
        List<SmsCounter> List();
        List<SMSMessageQueueInfo> GetBy(TypesSmsProxyTypes proxy, string status, int page);
    }
}
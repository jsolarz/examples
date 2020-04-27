
using System.Collections.Generic;
using BusinessLogic.Billing.SMS.ATSS;
using Framework.Core;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsServiceConfiguration
    {
        dynamic AppSettings { get; }
    }
}
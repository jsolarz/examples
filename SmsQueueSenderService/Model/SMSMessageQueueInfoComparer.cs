using BusinessLogic.Billing.SMS.ATSS;
using System.Collections.Generic;

namespace SmsQueueSenderService.Model
{
    public class SMSMessageQueueInfoComparer : IEqualityComparer<SMSMessageQueueInfo>
    {
        public bool Equals(SMSMessageQueueInfo x, SMSMessageQueueInfo y)
        {
            return x.Token == y.Token;
        }

        public int GetHashCode(SMSMessageQueueInfo obj)
        {
            return obj.Token.GetHashCode();
        }
    }
}

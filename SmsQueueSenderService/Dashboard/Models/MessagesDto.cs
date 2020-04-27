using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activetrail.SmsQueueSenderService.Dashboard.Models
{
    public class MessagesDto
    {
        public List<SMSMessageQueueInfo> Messages { get; set; }
        
    }
}

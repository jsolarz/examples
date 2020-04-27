using ActiveTrail.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activetrail.SmsQueueSenderService.Dashboard.Models
{
    public class SmsCounter
    {
        public TypesSmsProxyTypes Proxy { get; set; }
        public int Total { get; set; }
        public int TotalSent { get; set; }
        public int TotalLastHour { get; set; }
        public int TotalLastDay { get; set; }
        public int TotalLastMonth { get; set; }
        public int AveragePerHour { get; set; }
        public int AveragePerDay { get; set; }
        public int AveragePerMonth { get; set; }
    }
}

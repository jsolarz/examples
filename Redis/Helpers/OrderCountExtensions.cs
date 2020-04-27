using .Redis.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Helpers
{
    public static class OrderCountExtensions
    {
        public static void Replace(this OrderCount oc, ProductMetrics item)
        {
            int index;
            if ((index = oc.Metrics.FindIndex(x => x.Sku == item.Sku)) > -1)
            {
                oc.Metrics[index] = item;
            }
        }
    }
}

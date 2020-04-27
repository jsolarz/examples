using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using .Redis.Helpers;

namespace Redis.Dtos
{
  public class OrderCount
  {
    public List<ProductMetrics> Metrics { get; set; }


    public static void AddMetric(string sku, CounterEnum counterType = CounterEnum.None)
    {
      var connector = new Connector(new ConnectorSettingsFactory());
      if (!connector.Exists(Constants.CacheKeys.ORDER_COUNTS))
      {
        connector.Add<OrderCount>(Constants.CacheKeys.ORDER_COUNTS,
            new OrderCount
            {
              Metrics = new List<ProductMetrics>()
            },
            DateTime.Now.AddDays(1)
        );
      }

      OrderCount counts = new Connector(new ConnectorSettingsFactory()).Get<OrderCount>(Constants.CacheKeys.ORDER_COUNTS);

      ProductMetrics metrics = counts.Metrics.SingleOrDefault(x => x.Sku == sku);
      if (metrics == null)
      {
        metrics = new ProductMetrics
        {
          Sku = sku,
          New = 1,
          Redeemed = 0,
          RedeemedAndPaid = 0,
          Paid = 0,
        };
        counts.Metrics.Add(metrics);
      }
      else
      {
        switch (counterType)
        {
          case CounterEnum.New:
            metrics.New = metrics.New + 1;
            break;
          case CounterEnum.Redeemed:
            metrics.Redeemed = metrics.Redeemed + 1;
            metrics.New = metrics.New - 1;
            break;
          case CounterEnum.Paid:
            metrics.Paid = metrics.Paid + 1;
            metrics.Redeemed = metrics.Redeemed - 1;
            break;
          case CounterEnum.RedeemedAndPaid:
            metrics.RedeemedAndPaid = metrics.RedeemedAndPaid + 1;
            break;
        }
        counts.Replace(metrics);
      }

      metrics.RecalculateTotal();

      connector.Add<OrderCount>(Constants.CacheKeys.ORDER_COUNTS, counts, DateTime.Now.AddDays(Constants.CACHE_EXPIRATION));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Dtos
{
    public class ProductMetrics
    {
        public string Sku { get; set; }
        public int New { get; set; }
        public int Redeemed { get; set; }
        public int Paid { get; set; }
        public int RedeemedAndPaid { get; set; }
        public int Total { get; set; }

        public ProductMetrics()
        {
            this.New = 0;
            this.Redeemed = 0;
            this.Paid = 0;
            this.RedeemedAndPaid = 0;
            this.Total = 0;
        }

        public void RecalculateTotal()
        {
            this.Total = this.New + this.Redeemed + this.Paid + this.RedeemedAndPaid;
        }
    }
}

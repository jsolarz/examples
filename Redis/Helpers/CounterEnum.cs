using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Helpers
{
    public enum CounterEnum
    {
        None,
        New,
        Redeemed,
        Paid,
        RedeemedAndPaid
    }
}

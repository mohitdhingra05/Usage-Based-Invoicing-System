using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usage_Based_Invoicing_System.Model
{

    public static class BillingSettings
    {
        public const int ApiFreeTier = 10000;
        public const decimal ApiRate = 0.00015m;
        public const int StorageFreeTier = 50;
        public const decimal StorageRate = 0.20m;
        public const int ComputeFreeTier = 200;
        public const decimal ComputeRate = 0.08m;
    }
}

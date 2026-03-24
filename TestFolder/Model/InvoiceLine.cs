using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usage_Based_Invoicing_System.Model
{
    public class InvoiceLine
    {
        public string CustomerId { get; init; } = "";
        public decimal ApiCost { get; init; }
        public decimal StorageCost { get; init; }
        public decimal ComputeCost { get; init; }
        public decimal Total => ApiCost + StorageCost + ComputeCost;
    }
}

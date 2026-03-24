using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Usage_Based_Invoicing_System.Model
{
    public class UsageRecord
    {
        public string CustomerId { get; set; } = "";
        public JsonElement API_Calls { get; set; }
        public JsonElement Storage_GB { get; set; }
        public JsonElement Compute_Minutes { get; set; }
    }
}


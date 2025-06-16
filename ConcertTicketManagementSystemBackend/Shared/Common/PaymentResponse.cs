using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentReference { get; set; }
    }
}

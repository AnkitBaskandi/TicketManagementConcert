﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class PaymentRequest
    {
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }
}

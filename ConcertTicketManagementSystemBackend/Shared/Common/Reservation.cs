using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string Status { get; set; }
    }
}

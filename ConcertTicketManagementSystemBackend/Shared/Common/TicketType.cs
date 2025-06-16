using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class TicketType
    {
        public int TicketTypeId { get; set; }
        public int EventId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }
}

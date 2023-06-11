using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWorkings.Events
{
    public struct InventoryItemAddedEvent
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}

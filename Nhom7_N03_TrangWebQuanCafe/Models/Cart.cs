using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int? Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }
        public int? SalesReceiptId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
        public virtual SalesReceipt? SalesReceipt { get; set; }
    }
}

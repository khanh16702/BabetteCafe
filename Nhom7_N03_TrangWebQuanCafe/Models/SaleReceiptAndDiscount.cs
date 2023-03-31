using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class SaleReceiptAndDiscount
    {
        public int? SalesReceiptId { get; set; }
        public int? DiscountCodeId { get; set; }

        public virtual DiscountCode? DiscountCode { get; set; }
        public virtual SalesReceipt? SalesReceipt { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class SalesReceipt
    {
        public SalesReceipt()
        {
            Carts = new HashSet<Cart>();
        }

        public int SalesReceiptId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDelivered { get; set; }
        public double? ShippingFee { get; set; }
        public int? StaffId { get; set; }

        public virtual staff? Staff { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
    }
}

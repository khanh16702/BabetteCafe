using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class DiscountCode
    {
        public int DiscountCodeId { get; set; }
        public double? DecreaseAmount { get; set; }
        public double? MinimumToApply { get; set; }
        public string? Name { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class TableShop
    {
        public int TableId { get; set; }
        public int? Slots { get; set; }
        public string? Status { get; set; }
        public int? BookTimeId { get; set; }

        public virtual BookTime? BookTime { get; set; }
    }
}

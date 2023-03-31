using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class ImportReceiptDetail
    {
        public int ImportReceiptDetailId { get; set; }
        public int? Quantity { get; set; }
        public int? ImportReceiptId { get; set; }
        public int? ImportedGoodId { get; set; }

        public virtual ImportReceipt? ImportReceipt { get; set; }
        public virtual ImportedGood? ImportedGood { get; set; }
    }
}

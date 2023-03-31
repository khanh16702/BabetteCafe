using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class ImportedGood
    {
        public ImportedGood()
        {
            ImportReceiptDetails = new HashSet<ImportReceiptDetail>();
        }

        public int ImportedGoodId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; }
    }
}

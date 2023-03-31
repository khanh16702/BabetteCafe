using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class ImportReceipt
    {
        public ImportReceipt()
        {
            ImportReceiptDetails = new HashSet<ImportReceiptDetail>();
        }

        public int ImportReceiptId { get; set; }
        public DateTime? ImportDate { get; set; }
        public int? StaffId { get; set; }
        public int? ProviderId { get; set; }

        public virtual ProviderSide? Provider { get; set; }
        public virtual staff? Staff { get; set; }
        public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; }
    }
}

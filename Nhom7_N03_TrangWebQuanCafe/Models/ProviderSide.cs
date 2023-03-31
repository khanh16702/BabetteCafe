using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class ProviderSide
    {
        public ProviderSide()
        {
            ImportReceipts = new HashSet<ImportReceipt>();
        }

        public int ProviderId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<ImportReceipt> ImportReceipts { get; set; }
    }
}

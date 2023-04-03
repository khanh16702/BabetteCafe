using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class staff
    {
        public staff()
        {
            SalesReceipts = new HashSet<SalesReceipt>();
        }

        public int StaffId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<SalesReceipt> SalesReceipts { get; set; }
    }
}

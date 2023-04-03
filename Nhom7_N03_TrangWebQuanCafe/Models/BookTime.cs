using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class BookTime
    {
        public BookTime()
        {
            TableShops = new HashSet<TableShop>();
        }

        public int BookTimeId { get; set; }

        public virtual ICollection<TableShop> TableShops { get; set; }
    }
}

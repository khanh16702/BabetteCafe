using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class ProductAndCategory
    {
        public int? ProductId { get; set; }
        public int? ProductCategoryId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ProductCategory? ProductCategory { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class NewsAndCategory
    {
        public int? NewsId { get; set; }
        public int? NewsCategoryId { get; set; }

        public virtual News? News { get; set; }
        public virtual NewsCategory? NewsCategory { get; set; }
    }
}

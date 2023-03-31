using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class NewsAndTag
    {
        public int? NewsId { get; set; }
        public int? NewsTagId { get; set; }

        public virtual News? News { get; set; }
        public virtual NewsTag? NewsTag { get; set; }
    }
}

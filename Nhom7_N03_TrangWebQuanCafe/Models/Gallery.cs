using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class Gallery
    {
        public int GalleryId { get; set; }
        public string? Path { get; set; }
        public string? Format { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
        }

        public int ProductId { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Image { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}

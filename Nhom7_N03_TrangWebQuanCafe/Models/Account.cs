using System;
using System.Collections.Generic;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class Account
    {
        public Account()
        {
            Carts = new HashSet<Cart>();
            Galleries = new HashSet<Gallery>();
            News = new HashSet<News>();
            SalesReceipts = new HashSet<SalesReceipt>();
        }

        public int AccountId { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Introduction { get; set; }
        public string? Image { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Gallery> Galleries { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<SalesReceipt> SalesReceipts { get; set; }
    }
}

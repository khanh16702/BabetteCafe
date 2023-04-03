namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public class CartView
    {
        public int CartId { get; set; }
        public int? Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }
        public int? SalesReceiptId { get; set; }
        public double? ProductPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string?  Name { get; set; }
        public string? Image { get; set; }
    }
}

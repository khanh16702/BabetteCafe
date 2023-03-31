namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public class CategoryView
    {
        public int ProductCategoryId { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int NumberOfProducts { get; set; }
    }
}

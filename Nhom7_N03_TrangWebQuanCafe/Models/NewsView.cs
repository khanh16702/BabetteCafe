namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public class NewsView
    {
        public int NewsId { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public bool? IsIntroduction { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public int? AccountId { get; set; }
        public string? Image { get; set; }

        public string? Categories { get; set; }
        public string? Tags { get; set; }
        public string CustomDate { get; set; }
    }
}

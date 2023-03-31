namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public class AccountView
    {
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

        public string? RoleName { get; set; }
        public string? RetypedPassword { get; set; }
    }
}

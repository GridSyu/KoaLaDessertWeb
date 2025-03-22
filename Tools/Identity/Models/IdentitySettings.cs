namespace KoaLaDessertWeb.Tools.Identity.Models
{
    public class IdentitySettings
    {
        public string[] Roles { get; set; }
        public AdminUserSettings AdminUser { get; set; }
    }

    public class AdminUserSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
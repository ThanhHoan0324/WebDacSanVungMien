namespace WebDacSanVungMien.Models
{
    public class Role
    {
        public int RoleID { get; set; } // PK
        public string RoleName { get; set; } // VD: "Admin", "Member"

        // Navigation
        public ICollection<User> Users { get; set; }
    }
}

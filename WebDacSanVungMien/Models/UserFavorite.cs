namespace WebDacSanVungMien.Models
{
    public class UserFavorite
    {
        // Có thể dùng Khóa tổng hợp (UserID, SpecialtyID) làm PK, hoặc thêm FavoriteID
        public int FavoriteID { get; set; } // PK
        public int UserID { get; set; } // FK
        public int SpecialtyID { get; set; } // FK
        public DateTime DateAdded { get; set; }

        // Navigation
        public User User { get; set; }
        public Specialty Specialty { get; set; }
    }
}

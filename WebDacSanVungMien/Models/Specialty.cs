namespace WebDacSanVungMien.Models
{
    public class Specialty
    {
        public int SpecialtyID { get; set; } // PK
        public int RegionID { get; set; } // FK
        public string SpecialtyName { get; set; }
        public string ShortDescription { get; set; } // Phục vụ cho trang danh sách/tìm kiếm
        public string Ingredients { get; set; }
        public string Preparation { get; set; }
        public string ImageURL { get; set; }
        public decimal AvgRating { get; set; }
        public int ViewCount { get; set; } // Thống kê lượt xem
        public bool IsApproved { get; set; } // Dùng cho Quản lý thông tin đặc sản (kiểm duyệt)

        // Navigation
        public Region Region { get; set; }
        public ICollection<UserFavorite> UserFavorites { get; set; }
        public ICollection<Rating> Ratings { get; set; } 
    }
}

namespace WebDacSanVungMien.Models
{
    public class Region
    {
        public int RegionID { get; set; } // PK
        public string RegionName { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; } // Dùng cho quản lý (Admin có thể ẩn/hiện vùng miền)
        public DateTime CreatedDate { get; set; } // Thông tin quản lý

        // Navigation
        public ICollection<Specialty> Specialties { get; set; }
    }
}

namespace WebDacSanVungMien.Models
{
    public class Rating
    {
        public int RatingID { get; set; } // PK
        public int UserID { get; set; } // FK - Ai đánh giá
        public int SpecialtyID { get; set; } // FK - Món nào được đánh giá
        public int Score { get; set; } // Điểm đánh giá (ví dụ: từ 1 đến 5)
        public string Comment { get; set; } // Nội dung đánh giá
        public DateTime RatingDate { get; set; }

        // Navigation
        public User User { get; set; }
        public Specialty Specialty { get; set; }
    }
}

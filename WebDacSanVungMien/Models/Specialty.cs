
﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Có thể cần cho [Column]

namespace WebDacSanVungMien.Models


{
    public class Specialty
    {
        public int SpecialtyID { get; set; } // PK


        [Required(ErrorMessage = "Vùng miền không được để trống.")]
        public int RegionID { get; set; } // FK

        [Required(ErrorMessage = "Tên đặc sản không được để trống.")]
        [MaxLength(255)]
        public string SpecialtyName { get; set; }

        [Required(ErrorMessage = "Mô tả ngắn không được để trống.")]
        public string ShortDescription { get; set; } // Phục vụ cho trang danh sách/tìm kiếm

        public string Ingredients { get; set; } // Nguyên liệu
        public string Preparation { get; set; } // Cách chế biến

        public string Description { get; set; }

        public string ?ImageURL { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Định dạng tiền tệ/giá
        public decimal Price { get; set; } // Thêm trường Price (Giá)


        public int RegionID { get; set; } // FK
        public string SpecialtyName { get; set; }
        public string ShortDescription { get; set; } // Phục vụ cho trang danh sách/tìm kiếm
        public string Ingredients { get; set; }
        public string Preparation { get; set; }
        public string ImageURL { get; set; }


        [Required]
        public DateTime CreatedDate { get; set; } // Thêm CreatedDate để quản lý

        // Navigation
        [ValidateNever]
        public Region Region { get; set; }

        [ValidateNever]
        public ICollection<UserFavorite> UserFavorites { get; set; }

        [ValidateNever]
        public ICollection<Rating> Ratings { get; set; }
    }
}

        // Navigation
        public Region Region { get; set; }
        public ICollection<UserFavorite> UserFavorites { get; set; }
        public ICollection<Rating> Ratings { get; set; } 
    }
}


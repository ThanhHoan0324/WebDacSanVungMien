
﻿// File: Region.cs

using System.ComponentModel.DataAnnotations; // Thêm dòng này

namespace WebDacSanVungMien.Models
{
    public class Region
    {
        public int RegionID { get; set; }

        [Required(ErrorMessage = "Tên vùng miền không được để trống.")] 
        public string RegionName { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        [Required] 
        public DateTime CreatedDate { get; set; }

        // Navigation
        public ICollection<Specialty> Specialties { get; set; }
        public Region()
        {
            Specialties = new List<Specialty>();
        }
    }
}


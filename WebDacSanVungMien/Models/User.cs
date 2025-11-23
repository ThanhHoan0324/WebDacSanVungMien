    using System;
    using System.Collections.Generic;

    namespace WebDacSanVungMien.Models
    {
        public class User
        {
            public int UserID { get; set; } 
            public int RoleID { get; set; } 
            public string Username { get; set; } 
            public string PasswordHash { get; set; } 
            public string Email { get; set; } 
            public string PhoneNumber { get; set; } 

            public DateTime RegistrationDate { get; set; }
            public bool IsBanned { get; set; } 
            public DateTime LastLogin { get; set; } 
            public Role Role { get; set; }
            public ICollection<UserFavorite> UserFavorites { get; set; }
            public ICollection<Rating> Ratings { get; set; }
        }
    }
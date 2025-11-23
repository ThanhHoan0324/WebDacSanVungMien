using System.ComponentModel.DataAnnotations;

namespace WebDacSanVungMien.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } // Mật khẩu đầu vào (chưa hash)

        [Phone]
        public string PhoneNumber { get; set; }
    }
}

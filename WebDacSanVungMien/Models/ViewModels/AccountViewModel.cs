using System.ComponentModel.DataAnnotations;
using WebDacSanVungMien.Models.DTOs;

namespace WebDacSanVungMien.Models.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
        public LoginDto Login { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
        public RegisterDto Register { get; set; }
    }
}

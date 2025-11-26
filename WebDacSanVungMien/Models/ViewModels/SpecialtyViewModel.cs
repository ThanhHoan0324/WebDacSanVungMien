using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebDacSanVungMien.Models.ViewModels
{
    public class SpecialtyViewModel  : Specialty
    {
        // Thuộc tính này sẽ nhận file từ Form
        [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện.")]
        [NotMapped] // Không ánh xạ vào Database
        public IFormFile ImageFile { get; set; }
    }
}

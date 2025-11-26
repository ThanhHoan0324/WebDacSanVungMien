using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO; 
using System.Linq;
using WebDacSanVungMien.Models;
using WebDacSanVungMien.Models.ViewModels;


namespace WebDacSanVungMien.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor (Dependency Injection)
        public AdminController(DatabaseContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult QuanLyThongTinVungMien()
        {
            var regions = _context.Regions.OrderBy(r => r.RegionID).ToList();
            return View(regions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRegion(Region region)
        {
            // Kiểm tra tính hợp lệ của Model (dựa trên các [Required], v.v. trong Model)
            if (ModelState.IsValid)
            {
                // 1. Gán các giá trị mặc định/quản lý
                region.CreatedDate = DateTime.Now;

                // 2. Thêm vào DbSet và lưu vào database
                _context.Regions.Add(region);

                _context.SaveChanges();

                // 3. Chuyển hướng về trang danh sách sau khi thêm thành công
                return RedirectToAction("QuanLyThongTinVungMien");
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // Dùng Debug.WriteLine nếu bạn đã thêm using System.Diagnostics;
                    System.Diagnostics.Debug.WriteLine($"Model Error: {error.ErrorMessage}");
                }
            }

            // Nếu Model không hợp lệ, chuyển hướng lại trang quản lý (hoặc trả lại Modal)
            // Để đơn giản, ta chuyển hướng lại trang quản lý
            return RedirectToAction("QuanLyThongTinVungMien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRegion(Region region)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra xem Region có tồn tại không
                var existingRegion = _context.Regions.Find(region.RegionID);

                if (existingRegion == null)
                {
                    // Xử lý lỗi nếu không tìm thấy
                    return NotFound();
                }

                // 2. Cập nhật các thuộc tính
                existingRegion.RegionName = region.RegionName;
                existingRegion.Description = region.Description;
                existingRegion.IsVisible = region.IsVisible;
                // Giữ nguyên CreatedDate, chỉ thay đổi các trường người dùng nhập

                // 3. Đánh dấu là đã thay đổi và lưu
                _context.Regions.Update(existingRegion);
                _context.SaveChanges();

                // 4. Chuyển hướng về trang danh sách
                return RedirectToAction("QuanLyThongTinVungMien");
            }

            // Nếu Model không hợp lệ, chuyển hướng lại trang quản lý
            return RedirectToAction("QuanLyThongTinVungMien");
        }

        // Thường dùng POST hoặc HttpDelete, nhưng GET được dùng cho link đơn giản
        [HttpPost, ActionName("DeleteRegion")] // Tên Action là DeleteRegion
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRegionConfirmed(int id)
        {
            var region = _context.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có Đặc Sản nào liên quan không trước khi xóa (Tùy thuộc vào Database/EF Core setup)

            _context.Regions.Remove(region);
            _context.SaveChanges();

            return RedirectToAction("QuanLyThongTinVungMien");
        }


        // Quản Lý Thông Tin Đặc Sản Vùng Miền 
        [HttpGet]
        public IActionResult QuanLyThongTinDacSanVungMien()
        {
            // Lấy danh sách Đặc Sản, bao gồm thông tin Vùng Miền liên quan
            var specialties = _context.Specialties
                                        .Include(s => s.Region)
                                        .OrderByDescending(s => s.CreatedDate)
                                        .ToList();

            // Lấy danh sách vùng miền để dùng cho Dropdown trong Modal Thêm/Sửa
            ViewBag.Regions = _context.Regions.Where(r => r.IsVisible).OrderBy(r => r.RegionName).ToList();

            return View(specialties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddDacSan(SpecialtyViewModel viewModel)
        {
            // Kiểm tra file upload (Nếu bạn đã dùng [Required] trong ViewModel, bạn có thể bỏ check này, 
            // nhưng giữ lại để kiểm tra thêm Length là hợp lý)
            if (viewModel.ImageFile == null || viewModel.ImageFile.Length == 0)
            {
                // Nếu validation từ [Required] trong ViewModel không hoạt động, lệnh này sẽ thêm lỗi
                ModelState.AddModelError("ImageFile", "Vui lòng chọn một file ảnh hợp lệ.");
            }

            // ⚠️ LƯU Ý QUAN TRỌNG: Kiểm tra IsValid trước khi cố gắng sử dụng ImageFile
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. XỬ LÝ UPLOAD FILE
                    // Dòng này đảm bảo chúng ta chỉ upload nếu không có lỗi validation nào khác
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Lưu file vào thư mục wwwroot/images
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    // 2. GÁN DỮ LIỆU TỪ VIEWMODEL VÀO MODEL CHÍNH
                    var specialty = new Specialty
                    {
                        // Đảm bảo bạn đã truyền đủ các trường [Required] khác (Ingredients, Preparation, etc.)
                        SpecialtyName = viewModel.SpecialtyName,
                        RegionID = viewModel.RegionID,
                        ShortDescription = viewModel.ShortDescription,
                        Description = viewModel.Description,
                        Price = viewModel.Price,
                        Ingredients = viewModel.Ingredients,  // Cần đảm bảo có trong ViewModel
                        Preparation = viewModel.Preparation,  // Cần đảm bảo có trong ViewModel
                                                              // LƯU ĐƯỜNG DẪN TƯƠNG ĐỐI
                        ImageURL = "/img/" + uniqueFileName,
                        CreatedDate = DateTime.Now,
                        AvgRating = 0,
                        ViewCount = 0,
                        IsApproved = false,
                    };

                    // 3. LƯU VÀO DATABASE
                    _context.Specialties.Add(specialty);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Thêm đặc sản '{specialty.SpecialtyName}' thành công và đang chờ duyệt!";
                    return RedirectToAction("QuanLyThongTinDacSanVungMien"); // Thành công: Redirect
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi thêm đặc sản: {ex.Message}. Vui lòng kiểm tra log chi tiết.";
                    Debug.WriteLine($"Error adding specialty: {ex.Message}");
                }
            }

            // --- XỬ LÝ KHI MODELSTATE.ISVALID LÀ FALSE HOẶC LỖI TRY/CATCH ---

            // Tái tạo ViewBag.Regions để Modal vẫn có Dropdown
            ViewBag.Regions = _context.Regions.Where(r => r.IsVisible).OrderBy(r => r.RegionName).ToList();

            // Tái tạo danh sách Đặc Sản (Model) để View không bị lỗi
            var specialties = _context.Specialties
                                        .Include(s => s.Region)
                                        .OrderByDescending(s => s.CreatedDate)
                                        .ToList();

            // ⚠️ Trả về View chính cùng với ModelState (chứa lỗi) và danh sách Đặc Sản
            // Điều này đảm bảo ValidationSummary và các trường input bị lỗi được hiển thị.
            return View("QuanLyThongTinDacSanVungMien", specialties);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDacSan(Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                var existingSpecialty = _context.Specialties.Find(specialty.SpecialtyID);

                if (existingSpecialty == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy Đặc Sản cần cập nhật.";
                    return RedirectToAction("QuanLyThongTinDacSanVungMien");
                }

                try
                {
                    // Cập nhật các thuộc tính
                    existingSpecialty.SpecialtyName = specialty.SpecialtyName;
                    existingSpecialty.RegionID = specialty.RegionID;
                    existingSpecialty.ShortDescription = specialty.ShortDescription;
                    existingSpecialty.Description = specialty.Description; // Mô tả chi tiết
                    existingSpecialty.Ingredients = specialty.Ingredients;
                    existingSpecialty.Preparation = specialty.Preparation;
                    existingSpecialty.ImageURL = specialty.ImageURL;
                    existingSpecialty.Price = specialty.Price;
                    existingSpecialty.IsApproved = specialty.IsApproved; // Có thể thay đổi trạng thái duyệt

                    _context.Specialties.Update(existingSpecialty);
                    _context.SaveChanges();

                    // Thông báo thành công
                    TempData["SuccessMessage"] = $"Cập nhật đặc sản '{specialty.SpecialtyName}' thành công!";
                }
                catch (Exception ex)
                {
                    // Thông báo thất bại
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật đặc sản: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu cập nhật không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("QuanLyThongTinDacSanVungMien");
        }

        [HttpPost, ActionName("DeleteDacSan")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDacSanConfirmed(int id)
        {
            var specialty = _context.Specialties.Find(id);

            if (specialty == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy Đặc Sản để xóa.";
                return RedirectToAction("QuanLyThongTinDacSanVungMien");
            }

            try
            {
                // Note: EF Core sẽ xử lý việc xóa các ràng buộc nếu được cấu hình Cascade Delete.
                // Nếu không, bạn cần xóa các Rating và UserFavorite liên quan trước.
                // Tạm thời chỉ xóa Specialty:
                _context.Specialties.Remove(specialty);
                _context.SaveChanges();

                // Thông báo thành công
                TempData["SuccessMessage"] = $"Xóa đặc sản '{specialty.SpecialtyName}' thành công!";
            }
            catch (Exception ex)
            {
                // Thông báo thất bại (thường do ràng buộc khóa ngoại)
                TempData["ErrorMessage"] = $"Lỗi khi xóa đặc sản '{specialty.SpecialtyName}'. Có thể có dữ liệu liên quan (Rating, Favorite) cần xóa trước: {ex.Message}";
            }

            return RedirectToAction("QuanLyThongTinDacSanVungMien");
        }

        public IActionResult GetEditDacSanPartial(int id)
        {
            var specialty = _context.Specialties
                                    .Include(s => s.Region)
                                    .FirstOrDefault(s => s.SpecialtyID == id);

            if (specialty == null)
            {
                return NotFound("Không tìm thấy đặc sản.");
            }

            ViewBag.Regions = _context.Regions.Where(r => r.IsVisible).OrderBy(r => r.RegionName).ToList();

            return PartialView("_EditDacSanPartial", specialty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleApproval(int id, bool isApproved)
        {
            var existingSpecialty = await _context.Specialties.FindAsync(id);

            if (existingSpecialty == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đặc sản." });
            }

            try
            {
                existingSpecialty.IsApproved = isApproved;
                _context.Specialties.Update(existingSpecialty);
                await _context.SaveChangesAsync();

                return Json(new { success = true }); // Trả về JSON thành công
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ToggleApproval Error: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi cập nhật DB: {ex.Message}" });
            }
        }


        public IActionResult QuanLyThongTinThanhVien()
        {
            return View();
        }
        public IActionResult ThongKeDacSanYeuThich()
        {
            return View();
        }
        public IActionResult ThongKeThanhVien()
        {
            return View();
        }
    }
}

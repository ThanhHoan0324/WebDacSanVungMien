using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDacSanVungMien.Models.DTOs;
using WebDacSanVungMien.Models;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebDacSanVungMien.Models.ViewModels;

namespace WebDacSanVungMien.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;

        public AccountController(DatabaseContext context)
        {
            _context = context;
        }

        private const string AuthViewName = "Login"; // Tên View hợp nhất

        // --- HÀNH ĐỘNG GET: Hiển thị form Đăng Nhập/Đăng Ký HỢP NHẤT ---
        [HttpGet]
        public IActionResult Login()
        {
            var viewModel = new AccountViewModel
            {
                Login = new LoginDto(),
                Register = new RegisterDto()
            };
            ViewBag.ActiveTab = "Login";
            return View(AuthViewName, viewModel);
        }

        // --- HÀNH ĐỘNG POST: Xử lý Đăng Ký (Sửa để nhận AccountViewModel) ---
        [HttpPost]
        public async Task<IActionResult> Register(AccountViewModel viewModel) // <-- Đã cập nhật
        {
            // Bỏ qua Validation cho phần Login
            ModelState.Keys
                .Where(k => k.StartsWith("Login"))
                .ToList()
                .ForEach(k => ModelState.Remove(k));

            var model = viewModel.Register;

            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = "Register";
                return View(AuthViewName, viewModel);
            }

            // 1. Kiểm tra tồn tại Username hoặc Email
            if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                ModelState.AddModelError("Register.Email", "Username hoặc Email đã được sử dụng.");
                ViewBag.ActiveTab = "Register";
                return View(AuthViewName, viewModel);
            }

            // 2. Tạo đối tượng User và Hash mật khẩu
            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber ?? string.Empty,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleID = 2,
                RegistrationDate = DateTime.Now,
                IsBanned = false,
                LastLogin = DateTime.Now
            };

            _context.Users.Add(newUser);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account"); // <-- return ở đây
            }
            catch (DbUpdateException ex)
            {
                // Vẫn nên kiểm tra lỗi ở đây nếu Debug không tự hiển thị
                ModelState.AddModelError("", "Lỗi Database: " + ex.Message);
                ViewBag.ActiveTab = "Register";
                return View(AuthViewName, viewModel);
            }

        }

        // --- HÀNH ĐỘNG POST: Xử lý Đăng Nhập ---
        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel viewModel) // <-- Đã cập nhật
        {
            // Bỏ qua Validation cho phần Register
            ModelState.Keys
                .Where(k => k.StartsWith("Register"))
                .ToList()
                .ForEach(k => ModelState.Remove(k));

            var model = viewModel.Login;

            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = "Login";
                return View(AuthViewName, viewModel);
            }

            // 1. Tìm kiếm User bằng Email
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            // 2. Kiểm tra tồn tại và xác thực mật khẩu
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("Login.Email", "Email hoặc mật khẩu không đúng.");
                ViewBag.ActiveTab = "Login";
                return View(AuthViewName, viewModel);
            }

            // 3. Kiểm tra trạng thái bị cấm
            if (user.IsBanned)
            {
                ModelState.AddModelError("Login.Email", "Tài khoản của bạn đã bị khóa.");
                ViewBag.ActiveTab = "Login";
                return View(AuthViewName, viewModel);
            }

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // ✅ THÔNG BÁO THÀNH CÔNG ĐĂNG NHẬP
            // (Thêm code Authentication của bạn tại đây)
            // Sau khi hoàn tất Authentication:
            TempData["SuccessMessage"] = $"Chào mừng {user.Username} đã trở lại!";
            return RedirectToAction("Index", "Home");
        }
    }
}
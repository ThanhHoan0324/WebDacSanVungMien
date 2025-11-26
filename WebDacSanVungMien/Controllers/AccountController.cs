using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDacSanVungMien.Models.DTOs;
using WebDacSanVungMien.Models;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebDacSanVungMien.Models.ViewModels;
using System.Security.Claims; 
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies; 
namespace WebDacSanVungMien.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;
        // Giả định RoleID = 1 là Admin, RoleID = 2 là Member
        private const int AdminRoleID = 1;
        private const int MemberRoleID = 2;

        public AccountController(DatabaseContext context)
        {
            _context = context;
        }
        private const string AuthViewName = "Login";
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

        [HttpPost]
        public async Task<IActionResult> Register(AccountViewModel viewModel)
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
                RoleID = MemberRoleID, // Mặc định là Member (RoleID = 2)

                RegistrationDate = DateTime.Now,
                IsBanned = false,
                LastLogin = DateTime.Now
            };

            _context.Users.Add(newUser);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            catch (DbUpdateException ex)
            {

                ModelState.AddModelError("", "Lỗi Database: " + ex.Message);
                ViewBag.ActiveTab = "Register";
                return View(AuthViewName, viewModel);
            }
        }

        // --- HÀNH ĐỘNG POST: Xử lý Đăng Nhập (Đã sửa logic phân quyền) ---
        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel viewModel)
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
            // 1. Tìm kiếm User bằng Email, tải Role kèm theo

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
            // 4. Cập nhật thời gian đăng nhập cuối cùng
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // 5. THỰC HIỆN ĐĂNG NHẬP (Authentication)

            // Tạo Claims (Thông tin người dùng)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                // Thêm Claim Role dựa trên RoleName
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Guest")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // Thêm các thuộc tính khác như Persistent cookie, Expiration, v.v.
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Chào mừng {user.Username} đã trở lại!";

            // 6. PHÂN QUYỀN VÀ CHUYỂN HƯỚNG
            if (user.RoleID == AdminRoleID) // Nếu là Admin (RoleID = 1)
            {
                // Chuyển hướng đến trang Admin Dashboard
                return RedirectToAction("QuanLyThongTinVungMien", "Admin");
            }
            else // Nếu là User thường (Member, Guest, RoleID khác 1)
            {
                // Chuyển hướng đến trang chủ công cộng
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
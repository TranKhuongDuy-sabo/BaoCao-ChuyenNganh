using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.ViewModels;
using System.Security.Claims;

namespace project.Controllers
{
    public class AccountController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã có Session Username rồi thì đá về trang chủ luôn
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập không tồn tại!");
                return View(login);
            }

            if (user.Password != login.Password)
            {
                ModelState.AddModelError("Password", "Mật khẩu không đúng!");
                return View(login);
            }

            if (user.IsActive == false)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đang chờ Admin duyệt!");
                return View(login);
            }

            // --- LƯU THÔNG TIN VÀO SESSION ---
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "Customer");

            // Lưu thêm FullName để hiển thị "Xin chào..."
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);

            // Chuyển hướng
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("FullName");
            HttpContext.Session.Remove("Role");
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            // 1. Kiểm tra các điều kiện (Required, Length...)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. Kiểm tra xem Username đã tồn tại trong DB chưa
            var existingUser = db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng!");
                return View(model);
            }

            // 3. Kiểm tra Email trùng (nếu cần thiết)
            var existingEmail = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                return View(model);
            }

            // 4. Map dữ liệu từ RegisterModel sang User Entity
            var newUser = new project.Models.User
            {
                Username = model.Username,
                Password = model.Password,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Role = "Customer", // Mặc định tài khoản mới là Khách hàng
                IsActive = false
            };


            // 5. Lưu vào Database
            db.Users.Add(newUser);
            db.SaveChanges();

            // 6. Thông báo thành công và chuyển sang trang Login
            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng chờ Admin duyệt tài khoản để đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}

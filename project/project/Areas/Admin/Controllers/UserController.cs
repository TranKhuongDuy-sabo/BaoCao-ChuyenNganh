using Microsoft.AspNetCore.Mvc;
using project.Attributes;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class UserController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index()
        {
            return View(db.Users.ToList());
        }

        [HttpGet]
        public IActionResult sua(int id)
        {
            if (id == 0)
            {
                return NotFound(); // Không có ID thì báo lỗi
            }

            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound(); // Không tìm thấy User
            }

            return View("formSua",user); // Trả User về View để hiển thị
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(int id, User user)
        {
            ModelState.Remove("Password");
            ModelState.Remove("Username");

            // Tìm User cũ trong Database
            var userInDb = db.Users.Find(id);

            if (userInDb == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // CẬP NHẬT CÁC CỘT CHO PHÉP SỬA
                // Lưu ý: Không cập nhật Password và Username để tránh lỗi dữ liệu

                userInDb.FullName = user.FullName;
                userInDb.Phone = user.Phone;
                userInDb.Email = user.Email;
                userInDb.Address = user.Address;

                // QUAN TRỌNG: Cập nhật quyền (Admin/Customer)
                userInDb.Role = user.Role;

                // Lưu thay đổi
                db.SaveChanges();
                TempData["Success"] = "  Cập nhập tài khoản thành công!";
                // Quay về danh sách
                return RedirectToAction("Index");
            }

            // Nếu lỗi validation thì trả lại View để sửa
            return View("formSua",user);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();

            if (id == 1)
            {
                TempData["Error"] = "CẢNH BÁO: Bạn không được phép xóa tài khoản Super Admin này!";
                return RedirectToAction("Index");
            }

            var user = db.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id == 1)
            {
                return BadRequest("Không thể xóa Super Admin!");
            }

            var user = db.Users.Find(id);
            if (user == null) return NotFound();

            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["Success"] = " Đã xóa tài khoản thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu User này đã có đơn hàng (Khóa ngoại)
                // Nếu xóa User thì đơn hàng sẽ bị mất chủ -> SQL chặn lại
                ViewBag.Error = "Không thể xóa user này vì họ đã có phát sinh đơn hàng hoặc dữ liệu liên quan!";
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, bool trangThai)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                if (id == 1)
                {
                    return Json(new { success = false, message = "Không được khóa Super Admin!" });
                }

                user.IsActive = trangThai;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

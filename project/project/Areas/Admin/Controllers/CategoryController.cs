using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Attributes;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class CategoryController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index(string search)
        {
            // 1. Tạo Query
            var query = db.Categories.AsQueryable();

            // 2. Xử lý Tìm kiếm (Nếu có từ khóa)
            if (!string.IsNullOrEmpty(search))
            {
                // Tìm kiếm bất chấp dấu và hoa thường (Giống bên Product)
                query = query.Where(c => EF.Functions.Collate(c.CategoryName, "SQL_Latin1_General_CP1_CI_AI").Contains(search));

                // Lưu từ khóa để hiển thị lại
                ViewBag.Search = search;
            }

            // 3. Lấy dữ liệu
            var categories = query.ToList();

            return View(categories);
        }

        [HttpPost]
        public IActionResult them(Category cate)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(cate);
                db.SaveChanges();
                TempData["Success"] = "  Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(cate);
        }

        [HttpGet]
        public IActionResult formThem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult xoa(int id)
        {
            var cate = db.Categories.Find(id);
            if (cate == null)
            {
                return NotFound();
            }

            try
            {
                db.Categories.Remove(cate);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa danh mục thành công!";
            }
            catch (System.Exception)
            {
                TempData["Error"] = "Không thể xóa! Danh mục này đang chứa sản phẩm.";
                return View("formXoa", cate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult formXoa(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var cate = db.Categories.Find(id);
            if (cate == null)
            {
                return NotFound();
            }

            return View("formXoa", cate);
        }

        [HttpPost]
        public IActionResult sua(Category cate)
        {
            if (ModelState.IsValid)
            {
                var category = db.Categories.Find(cate.CategoryId);
                if (category != null)
                {
                    category.CategoryName = cate.CategoryName;
                    db.SaveChanges();
                    TempData["Success"] = "Đã cập nhật danh mục thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy danh mục để cập nhật!";
                }
                return RedirectToAction("Index");
            }
            return View("formSua", cate);
        }

        [HttpGet]
        public IActionResult formSua(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục cần sửa!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, bool trangThai)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                category.IsActive = trangThai;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

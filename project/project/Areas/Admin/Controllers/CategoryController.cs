using Microsoft.AspNetCore.Mvc;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index()
        {
            return View(db.Categories.ToList());
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
    }
}

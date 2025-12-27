using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Attributes;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class BrandController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index(string search)
        {
            // 1. Tạo Query
            var query = db.Brands.AsQueryable();

            // 2. Xử lý Tìm kiếm (Nếu có từ khóa)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    EF.Functions.Collate(b.BrandName, "SQL_Latin1_General_CP1_CI_AI").Contains(search) ||
                    EF.Functions.Collate(b.Origin, "SQL_Latin1_General_CP1_CI_AI").Contains(search)
                );

                // Lưu từ khóa để hiển thị lại
                ViewBag.Search = search;
            }

            // 3. Lấy dữ liệu
            var brands = query.ToList();

            return View(brands);
        }

        [HttpPost]
        public IActionResult them(Brand brand) 
        {
            if (ModelState.IsValid)
            {
                db.Brands.Add(brand);
                db.SaveChanges();
                TempData["Success"] = "Thêm thương hiệu thành công!";
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [HttpGet]
        public IActionResult formThem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult xoa(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return NotFound();
            }

            try
            {
                db.Brands.Remove(brand);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa thương hiệu thành công!";
            }
            catch (System.Exception)
            {
                TempData["Error"] = "Không thể xóa! Thương hiệu này đang chứa sản phẩm.";
                return View("formXoa", brand);
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

            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return NotFound();
            }

            return View("formXoa", brand);
        }

        [HttpPost]
        public IActionResult sua(Brand brand)
        {
            if (ModelState.IsValid)
            {
                var br = db.Brands.Find(brand.BrandId);
                if (br != null)
                {
                    br.BrandName = brand.BrandName;
                    br.Origin = brand.Origin;
                    db.SaveChanges();
                    TempData["Success"] = "Đã cập nhật thương hiệu thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy thương hiệu để cập nhật!";
                }
                return RedirectToAction("Index");
            }
            return View("formSua", brand);
        }

        [HttpGet]
        public IActionResult formSua(int id)
        {
            var category = db.Brands.Find(id);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy thương hiệu cần sửa!";
                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}

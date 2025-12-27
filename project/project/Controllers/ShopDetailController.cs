using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm cái này ?? dùng .Include
using project.Data; // Thêm cái này ?? dùng DbContext
using project.Models;

namespace project.Controllers
{
    public class ShopDetailController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Detail(int id)
        {
            if (id == 0) return NotFound();

            // 1. Lấy thông tin sản phẩm hiện tại
            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.Categories = db.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive == true)
                .ToList();

            ViewBag.RelatedProducts = db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id && p.IsActive == true)                  
                .OrderBy(x => Guid.NewGuid())                  
                .Take(4)                                         
                .ToList();

            return View(product);
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm cái này ?? dùng .Include
using project.Data; // Thêm cái này ?? dùng DbContext
using project.Models;

namespace project.Controllers
{
    public class HomeController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();

        public IActionResult Index()
        {
            var products = db.Products
                     .Include(p => p.Category)
                     .Where(p =>
                         p.IsActive == true &&
                         p.Category.IsActive == true
                     )
                     .ToList();

            return View(products);
        }

        public IActionResult Detail(int id)
        {
            if (id == 0) return NotFound();

            // 1. L?y s?n ph?m chi ti?t
            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            // 2. L?y danh m?c cho Sidebar bên trái
            ViewBag.Categories = db.Categories.Include(c => c.Products).ToList();

            // 3. L?y s?n ph?m liên quan (Cùng danh m?c, khác ID hi?n t?i, l?y 6 cái)
            ViewBag.RelatedProducts = db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                .Take(6)
                .ToList();

            return View(product);
        }

    }
}

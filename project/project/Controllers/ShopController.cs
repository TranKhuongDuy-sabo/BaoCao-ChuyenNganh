using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Để dùng .Include
using project.Data;
using project.Models;

namespace project.Controllers
{
    public class ShopController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index(int? categoryId)
        {
            // 1. Lấy danh sách danh mục cho Sidebar (Giữ nguyên)
            ViewBag.Categories = db.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive == true)
                .ToList();

            // 2. Tạo câu truy vấn cơ bản (Lấy sản phẩm đang bật)
            var query = db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive == true && p.Category.IsActive == true);

            // 3. NẾU CÓ CHỌN DANH MỤC -> THÌ LỌC THEO DANH MỤC ĐÓ
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 4. Lấy dữ liệu cuối cùng
            var products = query.ToList();

            // Lưu lại categoryId hiện tại để View biết mà tô màu đậm (Optional)
            ViewBag.CurrentCat = categoryId;

            return View(products);
        }
    }
}

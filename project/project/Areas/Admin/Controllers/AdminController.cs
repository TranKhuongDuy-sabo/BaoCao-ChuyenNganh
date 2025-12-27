using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Attributes;
using project.ViewModels; // Nhớ using cái ViewModel vừa tạo

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class AdminController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();

        public IActionResult Index()
        {
            // 1. Thống kê số lượng
            int countProduct = db.Products.Count();
            int countUser = db.Users.Count(); // Nếu chưa có bảng User thì xóa dòng này
            int countCategory = db.Categories.Count();
            int countBrand = db.Brands.Count();

            // 2. Lấy 5 sản phẩm mới nhất (sắp xếp theo ID giảm dần)
            var newProducts = db.Products
                                .Include(p => p.Category)
                                .OrderByDescending(p => p.ProductId)
                                .Take(5)
                                .ToList();

            // 3. Đóng gói vào ViewModel
            var model = new DashboardViewModel
            {
                TotalProducts = countProduct,
                TotalUsers = countUser,
                TotalCategories = countCategory,
                TotalBrands = countBrand,
                NewestProducts = newProducts
            };

            return View(model);
        }
    }
}
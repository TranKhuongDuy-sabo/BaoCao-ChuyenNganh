using project.Models;

namespace project.ViewModels
{
    public class DashboardViewModel
    {
        // Các số liệu thống kê
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }
        public int TotalUsers { get; set; }

        // Danh sách sản phẩm mới nhất để hiện lên bảng
        public List<Product> NewestProducts { get; set; }
    }
}
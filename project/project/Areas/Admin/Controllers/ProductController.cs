using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Attributes;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class ProductController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();

        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string search)
        {
            // 1. Tạo Query cơ bản (Kết nối bảng Category và Brand)
            var query = db.Products
                          .Include(p => p.Category)
                          .Include(p => p.Brand)
                          .AsQueryable(); // Chuyển sang IQueryable để ghép chuỗi lọc

            // 2. XỬ LÝ TÌM KIẾM 
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => EF.Functions.Collate(p.ProductName, "SQL_Latin1_General_CP1_CI_AI").Contains(search));

                // Lưu lại từ khóa để hiện lại trên ô tìm kiếm
                ViewBag.Search = search;
            }

            // 3. Thực thi và lấy danh sách
            var products = query.ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult formThem()
        {
            // Lấy danh sách Category và Brand để đổ vào Dropdown
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "BrandName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Tham số 'imageFile' là file ảnh người dùng chọn từ form
        public async Task<IActionResult> them(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // A. Xử lý lưu hình ảnh
                if (imageFile != null)
                {
                    // Tạo tên file ngẫu nhiên để không bị trùng (VD: uuid-iphone.jpg)
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Xác định đường dẫn: wwwroot/images/products
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Tạo thư mục nếu chưa có
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Copy file ảnh vào thư mục trên server
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Lưu tên file vào Database
                    product.Image = fileName;
                }
                else
                {
                    // Nếu user quên chọn ảnh, có thể báo lỗi hoặc gán ảnh mặc định
                    // product.Image = "no-image.jpg"; 
                    ModelState.AddModelError("Image", "Vui lòng chọn hình ảnh sản phẩm");
                }
            }

            // Kiểm tra lại ModelState sau khi xử lý ảnh
            if (ModelState.IsValid)
            {
                // B. Lưu vào Database
                db.Products.Add(product);
                await db.SaveChangesAsync();

                // C. Thông báo Success (sẽ hiện ở trang Index)
                TempData["Success"] = " Thêm sản phẩm mới thành công!";

                return RedirectToAction("Index");
            }

            // D. Nếu lỗi (VD: chưa nhập tên), phải nạp lại Dropdown để không bị lỗi View
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "BrandName", product.BrandId);

            return View("formThem" , product);
        }

        [HttpGet]
        public IActionResult formXoa(int id)
        {
            if (id == 0) return NotFound();

            var product = db.Products
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult xoa(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return NotFound();

            try
            {
                // A. Xóa ảnh trong thư mục wwwroot (Dọn rác)
                if (!string.IsNullOrEmpty(product.Image))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", product.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // B. Xóa trong Database
                db.Products.Remove(product);
                db.SaveChanges();

                TempData["Success"] = " Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // C. Bắt lỗi nếu sản phẩm đã có trong đơn hàng
                ViewBag.Error = "Không thể xóa sản phẩm này vì đã có khách mua hàng (Dính khóa ngoại)!";
                return View("formXoa" , product);
            }
        }

        [HttpGet]
        public IActionResult formSua(int id)
        {
            if (id == 0) return NotFound();

            var product = db.Products.Find(id);
            if (product == null) return NotFound();

            // Load danh mục và thương hiệu vào Dropdown, đồng thời chọn sẵn giá trị cũ
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "BrandName", product.BrandId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> sua(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Tìm sản phẩm cũ trong database để lấy thông tin ảnh cũ (nếu cần xóa)
                // Dùng AsNoTracking để tránh lỗi conflict khi Update
                var existingProduct = db.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == product.ProductId);

                if (imageFile != null)
                {
                    // A. NẾU CÓ CHỌN ẢNH MỚI

                    // 1. Tạo tên file mới
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // 2. Lưu ảnh mới lên server
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // 3. Xóa ảnh cũ đi (để dọn rác server)
                    if (existingProduct != null && !string.IsNullOrEmpty(existingProduct.Image))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", existingProduct.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // 4. Cập nhật tên ảnh mới vào model
                    product.Image = fileName;
                }
                else
                {
                    // B. NẾU KHÔNG CHỌN ẢNH MỚI -> Giữ nguyên tên ảnh cũ
                    // (Lưu ý: Phải có input hidden chứa ảnh cũ ở View thì product.Image mới có dữ liệu)
                    if (existingProduct != null)
                    {
                        product.Image = existingProduct.Image;
                    }
                }

                // Cập nhật vào Database
                db.Products.Update(product);
                await db.SaveChangesAsync();

                TempData["Success"] = " Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            // Nếu lỗi Validation -> Load lại Dropdown
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "BrandName", product.BrandId);

            return View("formSua", product);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, bool trangThai)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.IsActive = trangThai; // Cập nhật trạng thái
                db.SaveChanges(); // Lưu vào DB
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Text.Json; // Thư viện đọc/ghi JSON
using project.Attributes; // Namespace chứa Authentication 

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class ContactInfoController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ContactInfoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // 1. GET: Hiện form và điền sẵn dữ liệu cũ
        [HttpGet]
        public IActionResult Index()
        {
            var info = new ContactInfo();
            string path = Path.Combine(_env.WebRootPath, "contact.json");

            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                if (!string.IsNullOrEmpty(json))
                {
                    info = JsonSerializer.Deserialize<ContactInfo>(json);
                }
            }
            return View(info);
        }

        // 2. POST: Lưu dữ liệu mới vào file
        [HttpPost]
        public IActionResult CapNhat(ContactInfo model)
        {
            if (ModelState.IsValid)
            {
                // Biến object thành chuỗi JSON
                string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });

                // Ghi đè vào file cũ
                string path = Path.Combine(_env.WebRootPath, "contact.json");
                System.IO.File.WriteAllText(path, json);

                TempData["Success"] = "Cập nhật thông tin liên hệ thành công!";
            }
            return RedirectToAction("Index");
        }
    }
}
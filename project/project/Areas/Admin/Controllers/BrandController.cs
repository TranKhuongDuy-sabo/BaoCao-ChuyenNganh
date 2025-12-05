using Microsoft.AspNetCore.Mvc;
using project.Data;
using project.Models;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private SaBoTechContext db = new SaBoTechContext();
        public IActionResult Index()
        {
            return View(db.Brands.ToList());
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
    }
}

using Microsoft.AspNetCore.Mvc;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using project.Attributes;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

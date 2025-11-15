using Microsoft.AspNetCore.Mvc;

namespace project.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

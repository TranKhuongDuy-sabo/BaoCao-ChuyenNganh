using Microsoft.AspNetCore.Mvc;

namespace project.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

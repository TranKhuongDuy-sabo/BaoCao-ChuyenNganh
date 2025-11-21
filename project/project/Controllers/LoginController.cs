using Microsoft.AspNetCore.Mvc;

namespace project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

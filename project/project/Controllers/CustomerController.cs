using Microsoft.AspNetCore.Mvc;

namespace project.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

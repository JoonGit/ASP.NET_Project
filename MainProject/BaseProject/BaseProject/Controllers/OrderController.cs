using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

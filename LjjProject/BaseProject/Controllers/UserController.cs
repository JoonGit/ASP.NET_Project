using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

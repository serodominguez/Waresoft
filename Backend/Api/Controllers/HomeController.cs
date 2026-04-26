using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

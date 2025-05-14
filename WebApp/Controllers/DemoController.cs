using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class DemoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

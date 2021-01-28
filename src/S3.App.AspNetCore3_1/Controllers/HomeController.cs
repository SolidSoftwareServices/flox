using System.Diagnostics;
using System.Text;
using S3.App.AspNetCore3_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S3.App.AspNetCore3_1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            HttpContext.Session.Set("a",Encoding.ASCII.GetBytes("as"));
            return View();
        }

        public IActionResult About()
        {
            HttpContext.Session.TryGetValue("a",out byte[] b );

            var a = Encoding.ASCII.GetString(b);

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
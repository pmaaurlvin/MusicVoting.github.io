using Microsoft.AspNetCore.Mvc;
using MV4.Models;
using SpotifyAPI.Web;
using System.Diagnostics;

namespace MV4.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult lBZmN4PUDR2wXWnZMqQVOC8iU06avM6Q()
        {
            return View();
        }
        public IActionResult Loading()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }









    }
}
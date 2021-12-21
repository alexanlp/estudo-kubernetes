using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var client = new RestClient(new Uri(_configuration.GetSection("url").Value));
            var request = new RestRequest("weatherforecast", Method.GET, DataFormat.Json);

            var result = client.Execute<List<WeatherForecast>>(request);
            if (result != null)
            {
                return View(result.Data.First());     
            }
            
            return View();
        }

        public IActionResult Privacy()
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
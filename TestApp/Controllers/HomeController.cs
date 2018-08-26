using System;
using TestApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: OpenWeatherMapMvc
        public ActionResult Index()
        {
            var openWeatherMap = new OpenWeatherMap();

            return View(openWeatherMap);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string country, string city, string postalCode)
        {
            OpenWeatherMap openWeatherMap = null;

            if (city != null)
            {
                openWeatherMap = await GetWeatherAsync("api/weather/city/" + city + (country != null ? "?country=" + country : null));
            }
            else
            {
                openWeatherMap = await GetWeatherAsync("api/weather/postalcode/" + postalCode + (country != null ? "?country=" + country : null));
            }

            return View(openWeatherMap);
        }

        static async Task<OpenWeatherMap> GetWeatherAsync(string path)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:58617/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            OpenWeatherMap weather = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                weather = await response.Content.ReadAsAsync<OpenWeatherMap>();
            }

            return weather;
        }
    }
}
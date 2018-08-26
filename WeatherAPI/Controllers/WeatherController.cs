using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeeatherAPI.Models;

namespace WeeatherAPI.Controllers
{
    #region snippet_ControllerSignature
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    #endregion
    {
        // we use the OpenWeatherMap API http://openweathermap.org/api
        private const string apiKey = "337706442ac2c06ca7f6197f6c3555d1";

        #region snippet_GetByPostalCode
        [HttpGet("PostalCode/{postalCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OpenWeatherMap>> GetByPostalCodeAsync(string country, string postalCode)
        {
            var openWeatherMap = new OpenWeatherMap();

            var apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?zip=" + postalCode + (country != null ? "," + country : null) + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;

            await ProcessRequest(apiRequest, openWeatherMap);

            if (openWeatherMap == null || openWeatherMap.City == null)
            {
                return NotFound();
            }

            return openWeatherMap;
        }
        #endregion

        #region snippet_GetByCity
        [HttpGet("City/{city}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OpenWeatherMap>> GetByCityAsync(string country, string city)
        {
            var openWeatherMap = new OpenWeatherMap();

            var apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?q=" + city + (country != null ? "," + country : null) + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;

            await ProcessRequest(apiRequest, openWeatherMap);

            if (openWeatherMap == null)
            {
                return NotFound();
            }

            return openWeatherMap;
        }
        #endregion

        private async Task<ActionResult<OpenWeatherMap>> ProcessRequest(HttpWebRequest apiRequest, OpenWeatherMap openWeatherMap)
        {
            string apiResponse = "";

            try
            {
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    apiResponse = reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                return null;
            }

            // we use a tool for creating classes corresponding to the returned JSON http://json2csharp.com
            var rootObject = await Task.Run(() => JsonConvert.DeserializeObject<ResponseWeather>(apiResponse));

            // as part of our API we return a string containing the weather report
            openWeatherMap.Report = FormatRootObjectAsString(rootObject);

            // as part of our API we also return individual properties
            FormatRootObject(rootObject, ref openWeatherMap);

            return openWeatherMap;
        }

        private void FormatRootObject(ResponseWeather rootObject, ref OpenWeatherMap openWeatherMap)
        {
            openWeatherMap.City = rootObject.name;
            openWeatherMap.Country = rootObject.sys.country;
            openWeatherMap.WindSpeed = rootObject.wind.speed;
            openWeatherMap.Temperature = rootObject.main.temp;
            openWeatherMap.Humidity = rootObject.main.humidity;
            openWeatherMap.Conditions = rootObject.weather[0].description;
        }

        private string FormatRootObjectAsString(ResponseWeather rootObject)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table><tr><th>Weather Description</th></tr>");
            sb.Append("<tr><td>City:</td><td>" + rootObject.name + "</td></tr>");
            sb.Append("<tr><td>Country:</td><td>" + rootObject.sys.country + "</td></tr>");
            sb.Append("<tr><td>Wind:</td><td>" + rootObject.wind.speed + " Km/h</td></tr>");
            sb.Append("<tr><td>Temperature:</td><td>" + rootObject.main.temp + " °C</td></tr>");
            sb.Append("<tr><td>Humidity:</td><td>" + rootObject.main.humidity + "</td></tr>");
            sb.Append("<tr><td>Conditions:</td><td>" + rootObject.weather[0].description + "</td></tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
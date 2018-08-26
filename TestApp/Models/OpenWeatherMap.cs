using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class OpenWeatherMap
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public double WindSpeed { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Conditions { get; set; }

        public string Report { get; set; }
    }
}
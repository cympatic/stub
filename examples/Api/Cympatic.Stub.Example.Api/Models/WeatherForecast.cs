using System;
using System.Collections.Generic;

namespace Cympatic.Stub.Example.Api.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public List<WeatherForecastDetails> Details { get; } = new();
    }
}

using Cympatic.Stub.Example.Api.Enums;
using System;

namespace Cympatic.Stub.Example.Api.Models
{
    public class WeatherForecastDetails
    {
        public DateTime Date { get; set; }

        public ForecastDetailPosition Position { get; set; }

        public string Description { get; set; }

        public WeatherType Type { get; set; }

        public int WindSpeedKph { get; set; }

        public int WindSpeedMph => (int)Math.Round(WindSpeedKph / 1.609344, MidpointRounding.AwayFromZero);

        public int PrecipitationQuanty { get; set; }

        public int PrecipitationQuantityMm { get; set; }

        public decimal PrecipitationQuantityIn => Math.Round((decimal)(PrecipitationQuantityMm / 25.4), 1, MidpointRounding.AwayFromZero);

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

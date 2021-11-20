using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Extensions.Stub.SpecFlow.Attributes;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;
using Cympatic.Stub.Example.Api.SpecFlow.Enums;
using System;

namespace Cympatic.Stub.Example.Api.SpecFlow.Models
{
    [SpecFlowItemName("Weather forecast details")]
    public class WeatherForecastDetails : StubSpecFlowItem
    {
        public DateTime Date { get; set; }

        public ForecastDetailPosition Position { get; set; }

        public string Description { get; set; }

        public WeatherType Type { get; set; }

        public int WindSpeedKph { get; set; }

        public int WindSpeedMph => (int)Math.Round(WindSpeedKph / 1.609344, MidpointRounding.AwayFromZero);

        public string WindDirection { get; set; }

        public int PrecipitationQuantity { get; set; }

        public int PrecipitationQuantityMm { get; set; }

        public decimal PrecipitationQuantityIn => Math.Round((decimal)(PrecipitationQuantityMm / 25.4), 1, MidpointRounding.AwayFromZero);

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public override void ConnectSpecFlowItem(ISpecFlowItem item)
        {
            if (item is WeatherForecastComplex weatherForecast)
            {
                weatherForecast.AddDetails(this);
            }
        }
    }
}

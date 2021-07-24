using System.ComponentModel;

namespace Cympatic.Stub.Example.Api.Enums
{
    public enum WeatherType
    {
        None,

        [Description("Sunny/Clear")]
        Clear,

        [Description("Partially Cloudy")]
        PartiallyCloudy,

        Cloudy,

        Overcast,

        Rain,

        Drizzle,

        Hail,

        Snow,

        Stormy,

        Fog
    }
}

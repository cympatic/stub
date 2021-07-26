@Complex
Feature: WeatherForecast - Complex
	In order to validate that the Example.WebApi service gets pre-defined weahter forecasts are coming from a stub server
	As a developer
	I want to receive the pre-defined weather forecasts I added to the stub server
	And validate these received weather forecasts against the ones added to the stub server

Scenario: Receive pre-defined weather forecast
	Given multiple 'Weather forecast' items containing the following values
	| Date       | TemperatureC | Summary              |
	| 05-02-2021 | 1            | Scattered flurries   |
	| 06-02-2021 | 0            | Wind returns         |
	| 07-02-2021 | -3           | Late day snow        |
	| 08-02-2021 | -6           | Light snow           |
	| 09-02-2021 | -6           | Arctic air for a few |
	| 10-02-2021 | -7           | Cloudy and breezy    |
	| 11-02-2021 | -3           | Snow chance          |
	When the 'Stub Server' is prepared
	And the 'Weather forecast' service is requested for weather forecasts
	Then the request returned httpCode 'OK'
	And the request returned one or more 'WeatherForecast' items containing the following values
	| Date       | TemperatureC | Summary              |
	| 05-02-2021 | 1            | Scattered flurries   |
	| 06-02-2021 | 0            | Wind returns         |
	| 07-02-2021 | -3           | Late day snow        |
	| 08-02-2021 | -6           | Light snow           |
	| 09-02-2021 | -6           | Arctic air for a few |
	| 10-02-2021 | -7           | Cloudy and breezy    |
	| 11-02-2021 | -3           | Snow chance          |

Scenario: Receive pre-defined weather forecast with details
	Given multiple 'Weather forecast' items containing the following values
	| Date       | TemperatureC | Summary                         | Alias      |
	| 24-07-2021 | 27           | Clear with nightly rain showers | Forecast 1 |
	| 25-07-2021 | 30           | Thunderstorms chance            | Forecast 2 |
	And multiple 'Weather forecast details' items containing the following values
	| Weather forecast | Date       | Position  | Description  | Type         | WindSpeedKph | WindDirection | PrecipitationQuantity | PrecipitationQuantityMm | TemperatureC |
	| Forecast 1       | 24-07-2021 | Morning   | Clear        | Clear        | 5            | SSW           | 0                     | 0                       | 25           |
	| Forecast 1       | 24-07-2021 | Afternoon | Clear        | Clear        | 10           | SW            | 0                     | 0                       | 30           |
	| Forecast 1       | 24-07-2021 | Evening   | Clear        | Clear        | 10           | SW            | 0                     | 0                       | 27           |
	| Forecast 1       | 24-07-2021 | Night     | Rain showers | Rain         | 10           | SW            | 70                    | 3                       | 22           |
	| Forecast 2       | 25-07-2021 | Morning   | Thunder risk | Thunderstorm | 10           | SW            | 70                    | 2                       | 28           |
	| Forecast 2       | 25-07-2021 | Afternoon | Thunder risk | Thunderstorm | 10           | WSW           | 80                    | 2                       | 31           |
	| Forecast 2       | 25-07-2021 | Evening   | Thunder risk | Thunderstorm | 10           | WSW           | 80                    | 2                       | 30           |
	| Forecast 2       | 25-07-2021 | Night     | Thunder risk | Thunderstorm | 5            | ESE           | 90                    | 5                       | 24           |
	When the 'Stub Server' is prepared
	And the 'Weather forecast' service is requested for weather forecasts
	Then the request returned httpCode 'OK'

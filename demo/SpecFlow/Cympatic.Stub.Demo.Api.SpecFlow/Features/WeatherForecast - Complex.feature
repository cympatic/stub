@Complex
Feature: WeatherForecast - Complex
	In order to validate that the Demo.WebApi service gets pre-defined weahter forecasts are coming from a stub server
	As a developer
	I want to receive the pre-defined weather forecasts I added to the stub server
	And validate these received weather forecasts against the ones added to the stub server

Scenario: Receive pre-defined weather forecast
	Given multiple 'WeatherForecast Complex' items containing the following values
	| Date       | TemperatureC | Summary            | Alias |
	| 05-02-2021 | 1            | Scattered flurries | 1     |
	| 06-02-2021 | 0            | Wind returns       | 2     |
	| 07-02-2021 | -3           | Late day snow      | 3     |
	| 08-02-2021 | -6           | Light snow         | 4     |
	And multiple 'WeatherForecast Complex' items containing the following values
	| Date       | TemperatureC | Summary              | WeatherForecast Complex |
	| 09-02-2021 | -6           | Arctic air for a few | 1                       |
	| 10-02-2021 | -7           | Cloudy and breezy    | 1                       |
	| 11-02-2021 | -3           | Snow chance          | 1                       |
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

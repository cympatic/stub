Feature: WeatherForecast - Simple

	In order to validate that the Example.WebApi service gets weahter forecasts are coming from a stub server
	As a developer
	I want to receive the random weather forecasts I added to the stub server

Scenario: Receive random weather forecasts
	Given I have generate a random number of weahter forecasts
	And I have setup the response the webapi call
	When I request for weather forecasts 
	Then the result should be equal to the weather forecasts 
	And the stub server is called once

Feature: WeatherUnderground
	In order to lookup the weather from WeatherUnderground
	As a site user
	I want to be told the locations and the weather

@WeatherUnderground @weather
Scenario: Get Weather from WeatherUnderground
	Given I am in the city of San Francisco
	And I am in the state of CA
	And I am in the country of US
	And I selected WeatherUnderground as my weather provider
	When I get the forecast for the next 7 days
	Then The result should contain the current temperature

@WeatherUnderground
Scenario: Get Location by IP from WeatherUnderground
	Given I have an IP address of 74.103.166.10
	Then The location should return a city of Kennett Square

@WeatherUnderground
Scenario: Get List of Locations from WeatherUndeground
	Given I search for San Fran
	Then The results should contain the city of San Francisco
Feature: ForecastIo
	In order to lookup the weather from ForecastIo
	As a site user
	I want to be told the weather

@ForecastIo @weather
Scenario: Get Weather from ForecastIo
	Given I have the location with latitude of 39.86017227 and longitude of -75.70304871
	And I am in the state of CA
	And I am in the country of US
	And I selected ForecastIo as my weather provider
	When I get the forecast for the next 7 days
	Then The result should contain the current temperature

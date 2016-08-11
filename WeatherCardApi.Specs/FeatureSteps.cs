using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.ForecastIo.Services;
using WeatherCardApi.Provider.WeatherUnderground.Services;
using Xunit;

namespace WeatherCardApi.Specs {

	[Binding]
	public class FeatureSteps {
		private WeatherUndergroundService _weatherUndergroundService;
		private ForecastIoService _forecastIoService;
		private WeatherLocation _weatherLocation;
		private List<WeatherLocation> _weatherLocations;
		private WeatherRoot _weatherRoot;
		private string _weatherProvider;
		private string _weatherUndergroundBaseUri;
		private string _weatherUndergroundApiKey;
		private string _weatherUndergroundAutocompleteUri;
		private string _forecastIoBaseUri;
		private string _forecastIoApiKey;

		public FeatureSteps() {
			_weatherLocation = new WeatherLocation();
			_weatherRoot = new WeatherRoot();
		}

		[BeforeScenario("WeatherUnderground")]
		public void WeatherUndergroundInit() {
			_weatherUndergroundBaseUri = ConfigurationManager.AppSettings["WeatherUndergroundBaseUri"];
			_weatherUndergroundApiKey = ConfigurationManager.AppSettings["WeatherUndergroundApiKey"];
			_weatherUndergroundAutocompleteUri = ConfigurationManager.AppSettings["WeatherUndergroundAutocompleteUri"];

			_weatherUndergroundService = new WeatherUndergroundService(new WeatherUndergroundRequestService());
		}

		[BeforeScenario("ForecastIo")]
		public void ForecastIoInit() {
			_forecastIoBaseUri = ConfigurationManager.AppSettings["ForecastIoBaseUri"];
			_forecastIoApiKey = ConfigurationManager.AppSettings["ForecastIoApiKey"];

			_forecastIoService = new ForecastIoService(new ForecastIoRequestService());
		}

		[Given(@"I am in the city of (.*)")]
		public void GivenIAmInTheCityOf(string city) {
			_weatherLocation.city = city.Trim();
		}

		[Given(@"I am in the state of (.*)")]
		public void GivenIAmInTheStateOf(string state) {
			_weatherLocation.state = state.Trim();
		}

		[Given(@"I am in the country of (.*)")]
		public void GivenIAmInTheCountryOf(string country) {
			_weatherLocation.country = country.Trim();
		}

		[Given(@"I selected (.*) as my weather provider")]
		public void GivenISelectedAsMyWeatherProvider(string weatherProvider) {
			_weatherProvider = weatherProvider.Trim();
		}

		[When(@"I get the forecast for the next (.*) days")]
		public void WhenIGetTheForecastForTheNextDays(int days) {
			Task<WeatherRoot> task;

			if (_weatherProvider.Equals("weatherunderground", StringComparison.OrdinalIgnoreCase)) {
				task = _weatherUndergroundService.GetWeather(_weatherLocation, days, _weatherUndergroundBaseUri, _weatherUndergroundApiKey);
			} else if (_weatherProvider.Equals("forecastio", StringComparison.OrdinalIgnoreCase)) {
				task = _forecastIoService.GetWeather(_weatherLocation, days, _forecastIoBaseUri, _forecastIoApiKey);
			} else {
				return;
			}

			if (task.Result == null)
				return;
			_weatherRoot = task.Result;
		}

		[Then(@"The result should contain the current temperature")]
		public void ThenTheResultShouldContainTheCurrentTemperature() {
			Assert.Equal(false, (string.IsNullOrWhiteSpace(_weatherRoot.weatherCurrent.epoch)));
		}

		[Given(@"I have the location with latitude of (.*) and longitude of (.*)")]
		public void GivenIHaveTheLocationWithLatitudeOfAndLongitudeOf(string lat, string lon) {
			_weatherLocation.lat = lat;
			_weatherLocation.lon = lon;
		}

		[Given(@"I have an IP address of (.*)")]
		public void GivenIHaveAnIPAddressOf_(string ipAddress) {
			var task = _weatherUndergroundService.GetLocation(_weatherUndergroundBaseUri, _weatherUndergroundApiKey, ipAddress);
			if (task.Result == null)
				return;
			_weatherLocation = task.Result;
		}

		[Then(@"The location should return a city of (.*)")]
		public void ThenTheLocationShouldReturnACityOf(string city) {
			Assert.Equal(city, _weatherLocation.city);
		}

		[Given(@"I search for (.*)")]
		public void GivenISearchFor(string query) {
			var task = _weatherUndergroundService.GetLocations(_weatherUndergroundAutocompleteUri, query);
			if (task.Result == null)
				return;
			_weatherLocations = task.Result;
		}

		[Then(@"The results should contain the city of (.*)")]
		public void ThenTheResultsShouldContainTheCityOf(string city) {
			var searchResult = _weatherLocations.FirstOrDefault(l => l.city.Equals(city, StringComparison.OrdinalIgnoreCase));
			Assert.NotNull(searchResult);
		}
	}
}
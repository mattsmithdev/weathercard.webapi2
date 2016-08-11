using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.ForecastIo.Services;
using WeatherCardApi.Provider.WeatherUnderground.Services;
using WeatherCardApi.Services;
using Xunit;

namespace WeatherCardApi.Test.Web {

	public class WeatherServiceIntegrationTest {
		private readonly WeatherService _weatherService;
		private int _daysInForecast;
		private string _weatherUndergroundBaseUri;
		private string _weatherUndergroundApiKey;
		private string _weatherUndergroundAutocompleteUri;
		private string _forecastIoBaseUri;
		private string _forecastIoApiKey;
		private WeatherLocation _weatherLocationGood;

		public WeatherServiceIntegrationTest() {
			int.TryParse(ConfigurationManager.AppSettings["DaysInForecast"], out _daysInForecast);
			_weatherUndergroundBaseUri = ConfigurationManager.AppSettings["WeatherUndergroundBaseUri"];
			_weatherUndergroundApiKey = ConfigurationManager.AppSettings["WeatherUndergroundApiKey"];
			_weatherUndergroundAutocompleteUri = ConfigurationManager.AppSettings["WeatherUndergroundAutocompleteUri"];
			_forecastIoBaseUri = ConfigurationManager.AppSettings["ForecastIoBaseUri"];
			_forecastIoApiKey = ConfigurationManager.AppSettings["ForecastIoApiKey"];

			_weatherLocationGood = new WeatherLocation { city = "Kennett Square", state = "PA", country = "US", lat = "39.86017227", lon = "-75.70304871" };

			var weatherUndergroundRequestService = new WeatherUndergroundRequestService();
			var weatherUndergroundService = new WeatherUndergroundService(weatherUndergroundRequestService);

			var forecastIoRequestService = new ForecastIoRequestService();
			var forecastIoService = new ForecastIoService(forecastIoRequestService);

			_weatherService = new WeatherService(weatherUndergroundService, forecastIoService);
		}

		[Theory]
		[InlineData("74.103.166.10")]
		public async void GetLocationTest(string ipAddress) {
			var location = await _weatherService.GetLocation(ipAddress);

			Assert.NotNull(location);
			Assert.NotEqual("", location.city);
		}

		[Theory]
		[InlineData("San Francisco")]
		public async void GetLocationsTest(string query) {
			var locations = await _weatherService.GetLocations(query);
			Assert.NotNull(locations);
			Assert.NotEmpty(locations);
			Assert.Equal(false, string.IsNullOrWhiteSpace(locations[0].city));
		}

		[Theory]
		[InlineData("weatherunderground")]
		[InlineData("forecastio")]
		public async void GetWeatherTest(string provider) {
			var weatherRoot = await _weatherService.GetWeather(_weatherLocationGood, provider);
			Assert.NotNull(weatherRoot);

			Assert.NotNull(weatherRoot.weatherCurrent);
			Assert.Equal(false, string.IsNullOrWhiteSpace(weatherRoot.weatherCurrent.epoch));

			Assert.NotEmpty(weatherRoot.weatherDays);
			Assert.Equal(false, string.IsNullOrWhiteSpace(weatherRoot.weatherDays[0].epoch));

			Assert.NotEmpty(weatherRoot.weatherDays[0].weatherHourly);
			Assert.Equal(false, string.IsNullOrWhiteSpace(weatherRoot.weatherDays[0].weatherHourly[0].epoch));
		}

		[Theory]
		[InlineData("weatherunderground")]
		[InlineData("forecastio")]
		public async void GetWeatherBadLocationTest(string provider) {
			var weatherRoot = await _weatherService.GetWeather(new WeatherLocation(), provider);

			Assert.NotNull(weatherRoot);
			Assert.NotNull(weatherRoot.weatherCurrent);
			Assert.NotNull(weatherRoot.weatherDays);
			Assert.Equal(0, weatherRoot.weatherDays.Count);
		}
	}
}
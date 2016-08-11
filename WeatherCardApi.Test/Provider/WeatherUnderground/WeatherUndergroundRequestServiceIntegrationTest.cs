using System.Configuration;
using WeatherCardApi.Provider.WeatherUnderground.Services;
using Xunit;

namespace WeatherCardApi.Test.Provider.WeatherUnderground {

	public class WeatherUndergroundRequestServiceIntegrationTest {
		private readonly WeatherUndergroundRequestService _weatherUndergroundRequestService = new WeatherUndergroundRequestService();
		private readonly string _weatherUndergroundAutocompleteUri;

		public WeatherUndergroundRequestServiceIntegrationTest() {
			var weatherUndergroundBaseUri = ConfigurationManager.AppSettings["WeatherUndergroundBaseUri"];
			var weatherUndergroundApiKey = ConfigurationManager.AppSettings["WeatherUndergroundApiKey"];
			_weatherUndergroundRequestService.Init(weatherUndergroundBaseUri, weatherUndergroundApiKey);
			_weatherUndergroundAutocompleteUri = ConfigurationManager.AppSettings["WeatherUndergroundAutocompleteUri"];
		}

		[Theory]
		[InlineData("San Francisco")]
		public async void GetAutoCompleteTest(string query) {
			var autoComplete = await _weatherUndergroundRequestService.GetAutoComplete(_weatherUndergroundAutocompleteUri, query);
			Assert.NotNull(autoComplete);
			Assert.NotEmpty(autoComplete.RESULTS);
		}

		[Theory]
		[InlineData("!!~~")]
		public async void GetAutoCompleteBadQueryTest(string query) {
			var autoComplete = await _weatherUndergroundRequestService.GetAutoComplete(_weatherUndergroundAutocompleteUri, query);
			Assert.NotNull(autoComplete);
			Assert.Empty(autoComplete.RESULTS);
		}

		[Theory]
		[InlineData("74.103.166.10")]
		public async void GetCityStateCountryTest(string ip) {
			var location = await _weatherUndergroundRequestService.GetLocation(ip);
			Assert.NotNull(location);
			Assert.NotEqual("", location.city);
		}

		[Theory]
		[InlineData("BadIP")]
		public async void GetCityStateCountryBadIPTest(string ip) {
			var location = await _weatherUndergroundRequestService.GetLocation(ip);
			Assert.Null(location);
		}

		[Theory]
		[InlineData("PA", "Downingtown")]
		public async void GetCurrentObservationUSTest(string state, string city) {
			var currentObservationUS = await _weatherUndergroundRequestService.GetCurrentObservation(state, city);
			Assert.NotNull(currentObservationUS);
			Assert.NotEqual("", currentObservationUS.feelslike_string);
		}

		[Theory]
		[InlineData("", "Sydney", "AU")]
		public async void GetCurrentObservationNonUSTest(string state, string city, string country) {
			var currentObservationNonUS = await _weatherUndergroundRequestService.GetCurrentObservation(state, city, country);
			Assert.NotNull(currentObservationNonUS);
			Assert.NotEqual("", currentObservationNonUS.feelslike_string);
		}

		[Theory]
		[InlineData("PA", "Downingtown")]
		public async void GetHourlyForecastsUSTest(string state, string city) {
			var hourlyForecastsUS = await _weatherUndergroundRequestService.GetHourlyForecasts(state, city);
			Assert.NotNull(hourlyForecastsUS);
			Assert.NotEmpty(hourlyForecastsUS);
		}

		[Theory]
		[InlineData("", "Sydney", "AU")]
		public async void GetHourlyForecastsNonUSTest(string state, string city, string country) {
			var hourlyForecastsNonUS = await _weatherUndergroundRequestService.GetHourlyForecasts(state, city, country);
			Assert.NotNull(hourlyForecastsNonUS);
			Assert.NotEmpty(hourlyForecastsNonUS);
		}
	}
}
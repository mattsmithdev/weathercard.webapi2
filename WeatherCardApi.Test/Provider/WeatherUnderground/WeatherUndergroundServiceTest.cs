using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System.Collections.Generic;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.WeatherUnderground.Models.AutoComplete;
using WeatherCardApi.Provider.WeatherUnderground.Models.CurrentObservation;
using WeatherCardApi.Provider.WeatherUnderground.Models.Hourly10Day;
using WeatherCardApi.Provider.WeatherUnderground.Services;
using Xunit;

namespace WeatherCardApi.Test.Provider.WeatherUnderground {

	public class WeatherUndergroundServiceTest {
		private readonly IWeatherUndergroundRequestService _weatherUndergroundRequestService;

		public WeatherUndergroundServiceTest() {
			_weatherUndergroundRequestService = Substitute.For<IWeatherUndergroundRequestService>();
		}

		[Theory, AutoData]
		public void GetLocationsTest(string autocompleteUri, string query, AutoComplete autoComplete) {
			var fixture = new Fixture();
			autoComplete.RESULTS.ForEach(f => f.type = "city");
			autoComplete.RESULTS.ForEach(f => f.name = fixture.Create<string>() + ", " + fixture.Create<string>());
			_weatherUndergroundRequestService.GetAutoComplete(autocompleteUri, query).Returns(autoComplete);

			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var locationsTask = weatherUndergroundService.GetLocations(autocompleteUri, query);
			var locations = locationsTask.Result;

			Assert.NotNull(locations);
			Assert.NotEmpty(locations);
		}

		[Theory, AutoData]
		public void GetLocationsNoCitiesTest(string autocompleteUri, string query, AutoComplete autoComplete) {
			autoComplete.RESULTS.ForEach(f => f.type = "badstring");
			_weatherUndergroundRequestService.GetAutoComplete(autocompleteUri, query).Returns(autoComplete);

			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var locationsTask = weatherUndergroundService.GetLocations(autocompleteUri, query);
			var locations = locationsTask.Result;

			Assert.NotNull(locations);
			Assert.Empty(locations);
		}

		[Theory, AutoData]
		public void GetWeatherCurrentFromWeatherUndergroundTest(WeatherLocation location, CurrentObservation currentObservation) {
			_weatherUndergroundRequestService.GetCurrentObservation(location.state, location.city, location.country).Returns(currentObservation);

			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var weatherCurrentFromWeatherUndergroundTask = weatherUndergroundService.GetWeatherCurrentFromWeatherUnderground(location);
			var weatherCurrentFromWeatherUnderground = weatherCurrentFromWeatherUndergroundTask.Result;

			Assert.NotNull(weatherCurrentFromWeatherUnderground);
			Assert.NotEqual("", weatherCurrentFromWeatherUnderground.epoch);
		}

		[Theory, AutoData]
		public void GetWeatherCurrentFromWeatherUndergroundNoCityTest(WeatherLocation location, CurrentObservation currentObservation) {
			_weatherUndergroundRequestService.GetCurrentObservation(location.state, location.city, location.country).Returns(currentObservation);

			location.city = "";
			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var weatherCurrentFromWeatherUndergroundTask = weatherUndergroundService.GetWeatherCurrentFromWeatherUnderground(location);
			var weatherCurrentFromWeatherUnderground = weatherCurrentFromWeatherUndergroundTask.Result;

			Assert.NotNull(weatherCurrentFromWeatherUnderground);
			Assert.Equal(true, string.IsNullOrEmpty(weatherCurrentFromWeatherUnderground.epoch));
		}

		[Theory, AutoData]
		public void GetDailyFromWeatherUndergroundTest(WeatherLocation location, List<HourlyForecast> hourlyForecasts) {
			_weatherUndergroundRequestService.GetHourlyForecasts(location.state, location.city, location.country).Returns(hourlyForecasts);
			hourlyForecasts.ForEach(f => f.temp.english = "50");

			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var dailyHourlyFromWeatherUndergroundsTask = weatherUndergroundService.GetDailyHourlyFromWeatherUnderground(location, 7);
			var dailyHourlyFromWeatherUndergrounds = dailyHourlyFromWeatherUndergroundsTask.Result;

			Assert.NotNull(dailyHourlyFromWeatherUndergrounds);
			Assert.NotEmpty(dailyHourlyFromWeatherUndergrounds);
			Assert.Equal(false, string.IsNullOrEmpty(dailyHourlyFromWeatherUndergrounds[0].epoch));
		}

		[Theory, AutoData]
		public void GetHourlyFromWeatherUndergroundTest(WeatherLocation location, List<HourlyForecast> hourlyForecasts) {
			_weatherUndergroundRequestService.GetHourlyForecasts(location.state, location.city, location.country).Returns(hourlyForecasts);

			var weatherUndergroundService = new WeatherUndergroundService(_weatherUndergroundRequestService);
			var dailyHourlyFromWeatherUndergroundsTask = weatherUndergroundService.GetDailyHourlyFromWeatherUnderground(location, 7);
			var dailyHourlyFromWeatherUndergrounds = dailyHourlyFromWeatherUndergroundsTask.Result;

			Assert.NotNull(dailyHourlyFromWeatherUndergrounds);
			Assert.NotEmpty(dailyHourlyFromWeatherUndergrounds);
			Assert.NotEmpty(dailyHourlyFromWeatherUndergrounds[0].weatherHourly);
			Assert.Equal(false, string.IsNullOrEmpty(dailyHourlyFromWeatherUndergrounds[0].weatherHourly[0].epoch));
		}
	}
}
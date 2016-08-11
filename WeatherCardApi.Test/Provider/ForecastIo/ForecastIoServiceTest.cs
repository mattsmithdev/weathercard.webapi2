using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Linq;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.ForecastIo.Models;
using WeatherCardApi.Provider.ForecastIo.Services;
using Xunit;

namespace WeatherCardApi.Test.Provider.ForecastIo {

	public class ForecastIoServiceTest {
		private readonly IForecastIoRequestService _forecastIoRequestService;

		public ForecastIoServiceTest() {
			_forecastIoRequestService = Substitute.For<IForecastIoRequestService>();
		}

		[Theory, AutoData]
		public void GetWeatherCurrentFromForecastIoTest(WeatherLocation location, ForecastIoResponse forecastIoResponse) {
			_forecastIoRequestService.GetForecastIoResponse(location.lat, location.lon).Returns(forecastIoResponse);

			var forecastIoService = new ForecastIoService(_forecastIoRequestService);
			var getWeatherCurrentFromForecastIo = forecastIoService.GetWeatherCurrentFromForecastIo(forecastIoResponse);

			Assert.NotNull(getWeatherCurrentFromForecastIo);
			Assert.NotEqual("", getWeatherCurrentFromForecastIo.epoch);
		}

		[Theory, AutoData]
		public void GetWeatherCurrentFromForecastIoNoTimeTest(ForecastIoResponse forecastIoResponse) {
			forecastIoResponse.Currently.Time = 0;

			var forecastIoService = new ForecastIoService(_forecastIoRequestService);
			var getWeatherCurrentFromForecastIo = forecastIoService.GetWeatherCurrentFromForecastIo(forecastIoResponse);

			Assert.NotNull(getWeatherCurrentFromForecastIo);
			Assert.Equal(true, string.IsNullOrWhiteSpace(getWeatherCurrentFromForecastIo.epoch));
		}

		[Theory, AutoData]
		public void GetWeatherDailyFromForecastIo(ForecastIoResponse forecastIoResponse) {
			var forecastIoService = new ForecastIoService(_forecastIoRequestService);
			var getWeatherDailyHourlyFromForecastIo = forecastIoService.GetWeatherDailyHourlyFromForecastIo(forecastIoResponse, 7);

			Assert.NotNull(getWeatherDailyHourlyFromForecastIo);
			Assert.NotEmpty(getWeatherDailyHourlyFromForecastIo);
			Assert.Equal(false, string.IsNullOrEmpty(getWeatherDailyHourlyFromForecastIo[0].epoch));
		}

		[Theory, AutoData]
		public void GetWeatherHourlyFromForecastIo(ForecastIoResponse forecastIoResponse) {
			// Give a date range between two days.
			var r = new Random();
			forecastIoResponse.Daily.Data.ForEach(f => f.Time = r.Next(1470758311, 1470845115));
			forecastIoResponse.Hourly.Data.ForEach(f => f.Time = r.Next(1470758311, 1470845115));

			var forecastIoService = new ForecastIoService(_forecastIoRequestService);
			var getWeatherDailyHourlyFromForecastIo = forecastIoService.GetWeatherDailyHourlyFromForecastIo(forecastIoResponse, 7);

			Assert.NotNull(getWeatherDailyHourlyFromForecastIo);
			Assert.NotEmpty(getWeatherDailyHourlyFromForecastIo);

			// Grab any hourly
			var hourly = getWeatherDailyHourlyFromForecastIo.FirstOrDefault(h => h.weatherHourly != null);
			Assert.NotNull(hourly);
			Assert.Equal(false, hourly != null && string.IsNullOrEmpty(hourly.epoch));
		}
	}
}
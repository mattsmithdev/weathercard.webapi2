using NSubstitute;
using System;
using System.Net.Http;
using WeatherCardApi.Controllers;
using WeatherCardApi.Models;
using WeatherCardApi.Services;
using Xunit;

namespace WeatherCardApi.Test.Web {

	public class WeatherControllerTest {
		private readonly IWeatherService _iWeatherService;

		public WeatherControllerTest() {
			_iWeatherService = Substitute.For<IWeatherService>();
		}

		[Fact]
		public void GetTest() {
			var request = Substitute.For<HttpRequestMessage>();
			request.RequestUri = new Uri("http://localhost/api/weather/WeatherUnderground?city=Kennett%20Square&state=PA&country=US&lat=39.86017227&lon=-75.70304871");

			_iWeatherService.GetWeather(Arg.Is<WeatherLocation>(l => l.city == "Kennett Square"), "WeatherUnderground").Returns(new WeatherRoot());

			var weatherController = new WeatherController(_iWeatherService) { Request = request };

			var getTask = weatherController.Get("WeatherUnderground");

			var get = getTask.Result;

			Assert.NotNull(get.weatherCurrent);
		}
	}
}
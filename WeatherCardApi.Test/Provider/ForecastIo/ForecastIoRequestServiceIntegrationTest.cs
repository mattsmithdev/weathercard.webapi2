using System.Configuration;
using WeatherCardApi.Provider.ForecastIo.Services;
using Xunit;

namespace WeatherCardApi.Test.Provider.ForecastIo {

	public class ForecastIoRequestServiceIntegrationTest {
		private readonly ForecastIoRequestService _forecastIoRequestService = new ForecastIoRequestService();

		public ForecastIoRequestServiceIntegrationTest() {
			var weatherUndergroundBaseUri = ConfigurationManager.AppSettings["ForecastIoBaseUri"];
			var weatherUndergroundApiKey = ConfigurationManager.AppSettings["ForecastIoApiKey"];
			_forecastIoRequestService.Init(weatherUndergroundBaseUri, weatherUndergroundApiKey);
		}

		[Theory]
		[InlineData("0", "A")]
		public async void GetForecastIoResponseBadLatLonTest(string lat, string lon) {
			var getForecastIoResponse = await _forecastIoRequestService.GetForecastIoResponse(lat, lon);
			Assert.Null(getForecastIoResponse);
		}

		[Theory]
		[InlineData("39.86017227", "-75.70304871")]
		public async void GetForecastIoResponseTest(string lat, string lon) {
			var getForecastIoResponse = await _forecastIoRequestService.GetForecastIoResponse(lat, lon);
			Assert.NotNull(getForecastIoResponse);
			Assert.NotNull(getForecastIoResponse.Currently);
			Assert.Equal(false, string.IsNullOrWhiteSpace(getForecastIoResponse.Currently.Summary));
		}
	}
}
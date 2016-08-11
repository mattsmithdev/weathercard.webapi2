using System;
using System.Threading.Tasks;
using WeatherCardApi.Provider.ForecastIo.Models;
using WeatherCardApi.Utils;

namespace WeatherCardApi.Provider.ForecastIo.Services {

	public class ForecastIoRequestService : IForecastIoRequestService {
		private string _baseUri;

		public void Init(string baseUri, string apiKey) {
			_baseUri = $"{baseUri}/{apiKey}/";
		}

		public async Task<ForecastIoResponse> GetForecastIoResponse(string lat, string lon) {
			if ((string.IsNullOrWhiteSpace(lat)) || (string.IsNullOrWhiteSpace(lon)))
				return null;
			try {
				var path = $"{lat},{lon}?units=us&extend=hourly";
				var clientTask = (RestClient.Get<ForecastIoResponse>(_baseUri, path));
				var forecastIoResponse = await clientTask;
				return forecastIoResponse;
			} catch (Exception e) {
				Log.Error($"[GetForecastIoResponse]lat={lat}|lon={lon}", e);
			}
			return null;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.ForecastIo.Services;
using WeatherCardApi.Provider.WeatherUnderground.Services;

namespace WeatherCardApi.Services {

	public class WeatherService : IWeatherService {
		private readonly IWeatherUndergroundService _weatherUndergroundService;
		private readonly IForecastIoService _forecastIoService;
		private readonly int _daysInForecast;

		public WeatherService(IWeatherUndergroundService weatherUndergroundService, IForecastIoService forecastIoService) {
			_weatherUndergroundService = weatherUndergroundService;
			_forecastIoService = forecastIoService;
			int.TryParse(ConfigurationManager.AppSettings["DaysInForecast"], out _daysInForecast);
		}

		/// <summary>
		/// Gets the weather location information based upon an ip address
		/// </summary>
		/// <param name="ipAddress">IP address to lookup</param>
		/// <returns>Task of WeatherLocation. An empty WeatherLocation will be returned on error.</returns>
		public async Task<WeatherLocation> GetLocation(string ipAddress) {
			var weatherUndergroundBaseUri = ConfigurationManager.AppSettings["WeatherUndergroundBaseUri"];
			var weatherUndergroundApiKey = ConfigurationManager.AppSettings["WeatherUndergroundApiKey"];
			return await _weatherUndergroundService.GetLocation(weatherUndergroundBaseUri, weatherUndergroundApiKey, ipAddress);
		}

		/// <summary>
		/// Gets list of weather location information based upon a string query, such as San Fran
		/// </summary>
		/// <param name="query">string to query on</param>
		/// <returns>Task of List of WeatherLocation. An empty List of WeatherLocation will be returned on error.</returns>
		public async Task<List<WeatherLocation>> GetLocations(string query) {
			var weatherUndergroundAutocompleteUri = ConfigurationManager.AppSettings["WeatherUndergroundAutocompleteUri"];
			return await _weatherUndergroundService.GetLocations(weatherUndergroundAutocompleteUri, query);
		}

		/// <summary>
		/// Gets the weather for the passed location. Includes current and forecast weather.
		/// </summary>
		/// <param name="location">WeatherLocation</param>
		/// <param name="provider">Weather provider - weatherunderground or forecastio</param>
		/// <returns>Task of WeatherRoot</returns>
		public async Task<WeatherRoot> GetWeather(WeatherLocation location, string provider) {
			if (provider.Equals("weatherunderground", StringComparison.OrdinalIgnoreCase)) {
				var weatherUndergroundBaseUri = ConfigurationManager.AppSettings["WeatherUndergroundBaseUri"];
				var weatherUndergroundApiKey = ConfigurationManager.AppSettings["WeatherUndergroundApiKey"];

				return await _weatherUndergroundService.GetWeather(location, _daysInForecast, weatherUndergroundBaseUri, weatherUndergroundApiKey);
			}
			if (provider.Equals("forecastio", StringComparison.OrdinalIgnoreCase)) {
				var forecastIoBaseUri = ConfigurationManager.AppSettings["ForecastIoBaseUri"];
				var forecastIoApiKey = ConfigurationManager.AppSettings["ForecastIoApiKey"];

				return await _forecastIoService.GetWeather(location, _daysInForecast, forecastIoBaseUri, forecastIoApiKey);
			}
			return new WeatherRoot();
		}
	}
}
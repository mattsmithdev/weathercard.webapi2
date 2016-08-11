using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherCardApi.Models;

namespace WeatherCardApi.Provider.WeatherUnderground.Services {

	public interface IWeatherUndergroundService {

		Task<WeatherLocation> GetLocation(string baseUri, string apiKey, string ipAddress);

		Task<List<WeatherLocation>> GetLocations(string autocompleteUri, string query);

		Task<WeatherRoot> GetWeather(WeatherLocation location, int daysInForecast, string baseUri, string apiKey);
	}
}
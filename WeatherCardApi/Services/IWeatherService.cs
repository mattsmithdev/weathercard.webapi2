using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherCardApi.Models;

namespace WeatherCardApi.Services {

	public interface IWeatherService {

		Task<WeatherLocation> GetLocation(string ipAddress);

		Task<List<WeatherLocation>> GetLocations(string query);

		Task<WeatherRoot> GetWeather(WeatherLocation location, string provider);
	}
}
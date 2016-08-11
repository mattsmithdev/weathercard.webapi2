using System.Threading.Tasks;
using WeatherCardApi.Models;

namespace WeatherCardApi.Provider.ForecastIo.Services {

	public interface IForecastIoService {

		Task<WeatherRoot> GetWeather(WeatherLocation location, int daysInForecast, string baseUri, string apiKey);
	}
}
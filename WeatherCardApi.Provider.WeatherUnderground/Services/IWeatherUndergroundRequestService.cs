using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherCardApi.Provider.WeatherUnderground.Models.AutoComplete;
using WeatherCardApi.Provider.WeatherUnderground.Models.CurrentObservation;
using WeatherCardApi.Provider.WeatherUnderground.Models.Hourly10Day;
using WeatherCardApi.Provider.WeatherUnderground.Models.Location;

namespace WeatherCardApi.Provider.WeatherUnderground.Services {

	public interface IWeatherUndergroundRequestService {

		Task<AutoComplete> GetAutoComplete(string autocompleteUri, string query);

		void Init(string baseUri, string apiKey);

		Task<Location> GetLocation(string ipAddress);

		Task<CurrentObservation> GetCurrentObservation(string state, string city, string country);

		Task<List<HourlyForecast>> GetHourlyForecasts(string state, string city, string country);
	}
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeatherCardApi.Provider.WeatherUnderground.Models.AutoComplete;
using WeatherCardApi.Provider.WeatherUnderground.Models.CurrentObservation;
using WeatherCardApi.Provider.WeatherUnderground.Models.Hourly10Day;
using WeatherCardApi.Provider.WeatherUnderground.Models.Location;
using WeatherCardApi.Utils;

namespace WeatherCardApi.Provider.WeatherUnderground.Services {

	public class WeatherUndergroundRequestService : IWeatherUndergroundRequestService {
		private string _baseUri;

		public void Init(string baseUri, string apiKey) {
			_baseUri = $"{baseUri}/{apiKey}/";
		}

		public async Task<Location> GetLocation(string ipAddress) {
			if (string.IsNullOrWhiteSpace(ipAddress))
				return null;
			try {
				var path = "geolookup/q/autoip.json?geo_ip=" + ipAddress;
				var clientTask = (RestClient.Get<LocationRoot>(_baseUri, path));
				var locationRoot = await clientTask;
				if (locationRoot == default(LocationRoot))
					return null;
				var location = locationRoot.location;
				if (location != default(Location))
					return location;
			} catch (Exception e) {
				Log.Error($"[GetLocation]ipAddress={ipAddress}", e);
			}
			return null;
		}

		public async Task<AutoComplete> GetAutoComplete(string autocompleteUri, string query) {
			if (string.IsNullOrWhiteSpace(query))
				return null;
			try {
				var path = "/aq?query=" + query;
				var clientTask = (RestClient.Get<AutoComplete>(autocompleteUri, path));
				var autoComplete = await clientTask;
				return autoComplete;
			} catch (Exception e) {
				Log.Error($"[GetAutoComplete]ipAddress={autocompleteUri}|query={query}", e);
			}
			return null;
		}

		public async Task<CurrentObservation> GetCurrentObservation(string state, string city, string country = "US") {
			if (string.IsNullOrWhiteSpace(city))
				return null;
			try {
				var path = $"conditions/q/{country}/{state}/{city}.json";
				var clientTask = (RestClient.Get<CurrentObservationRoot>(_baseUri, path));
				var currentObservationRoot = await clientTask;
				if (currentObservationRoot == default(CurrentObservationRoot))
					return null;
				var currentObservation = currentObservationRoot.current_observation;
				return currentObservation;
			} catch (Exception e) {
				Log.Error($"[GetCurrentObservation]state={state}|city={city}|country={country}", e);
			}
			return null;
		}

		public async Task<List<HourlyForecast>> GetHourlyForecasts(string state, string city, string country = "US") {
			if (string.IsNullOrWhiteSpace(city))
				return null;
			try {
				var path = $"hourly10day/q/{country}/{state}/{city}.json";
				var clientTask = (RestClient.Get<Hourly10DayRoot>(_baseUri, path));
				var hourly10DayRoot = await clientTask;
				if (hourly10DayRoot == default(Hourly10DayRoot))
					return null;
				var hourlyForecasts = hourly10DayRoot.hourly_forecast;
				return hourlyForecasts;
			} catch (Exception e) {
				Log.Error($"[GetHourlyForecasts]state={state}|city={city}|country={country}", e);
			}
			return null;
		}
	}
}
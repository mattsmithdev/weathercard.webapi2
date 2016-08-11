using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeatherCardApi.Models;
using WeatherCardApi.Utils;

namespace WeatherCardApi.Provider.WeatherUnderground.Services {

	public class WeatherUndergroundService : IWeatherUndergroundService {
		private readonly IWeatherUndergroundRequestService _weatherUndergroundRequestService;
		private const string Scale = "F";

		public WeatherUndergroundService(IWeatherUndergroundRequestService weatherUndergroundRequestService) {
			_weatherUndergroundRequestService = weatherUndergroundRequestService;
		}

		/// <summary>
		/// Gets the weather for the passed location using city, state, and country. Includes current and forecast weather.
		/// </summary>
		/// <param name="location">WeatherLocation</param>
		/// <param name="daysInForecast">Number of days with 10 being the max</param>
		/// <param name="baseUri">WeatherUnderground base url</param>
		/// <param name="apiKey">WeatherUnderground api key</param>
		/// <returns>Task of WeatherRoot</returns>
		public async Task<WeatherRoot> GetWeather(WeatherLocation location, int daysInForecast, string baseUri, string apiKey) {
			var weatherRoot = new WeatherRoot { weatherLocation = location };

			_weatherUndergroundRequestService.Init(baseUri, apiKey);
			weatherRoot.weatherCurrent = await GetWeatherCurrentFromWeatherUnderground(location);
			weatherRoot.weatherDays = await GetDailyHourlyFromWeatherUnderground(location, daysInForecast);
			return weatherRoot;
		}

		/// <summary>
		/// Gets the weather location information based upon an ip address
		/// </summary>
		/// <param name="baseUri">WeatherUnderground base url</param>
		/// <param name="apiKey">WeatherUnderground api key</param>
		/// <param name="ipAddress">IP address to lookup</param>
		/// <returns>Task of WeatherLocation. An empty WeatherLocation will be returned on error.</returns>
		public async Task<WeatherLocation> GetLocation(string baseUri, string apiKey, string ipAddress) {
			var cacheKey = "WeatherUndergroundService_GetLocation_" + ipAddress;
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (WeatherLocation)cacheResult;

			_weatherUndergroundRequestService.Init(baseUri, apiKey);
			var weatherLocation = new WeatherLocation();
			var location = await _weatherUndergroundRequestService.GetLocation(ipAddress);
			if (string.IsNullOrWhiteSpace(location?.city))
				return weatherLocation;
			try {
				weatherLocation.city = location.city;
				weatherLocation.country = location.country;
				weatherLocation.lat = location.lat;
				weatherLocation.lon = location.lon;
				weatherLocation.state = location.state;
				weatherLocation.Display = (string.IsNullOrWhiteSpace(weatherLocation.state))
					? weatherLocation.city + ", " + weatherLocation.country : weatherLocation.city + ", " + weatherLocation.state;

				Cacher.Add(cacheKey, weatherLocation);
			} catch (Exception e) {
				Log.Error("[GetLocation]ipAddress={ipAddress}", e);
			}
			return weatherLocation;
		}

		/// <summary>
		/// Gets list of weather location information based upon a string query, such as San Fran
		/// </summary>
		/// <param name="autocompleteUri">WeatherUnderground autocomplete url</param>
		/// <param name="query">string to query on</param>
		/// <returns>Task of List of WeatherLocation. An empty List of WeatherLocation will be returned on error.</returns>
		public async Task<List<WeatherLocation>> GetLocations(string autocompleteUri, string query) {
			var cacheKey = "WeatherUndergroundService_GetLocations_" + Regex.Replace(query, @"[^A-Za-z0-9]+", "");
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (List<WeatherLocation>)cacheResult;

			var weatherLocations = new List<WeatherLocation>();

			var autoComplete = await _weatherUndergroundRequestService.GetAutoComplete(autocompleteUri, query);
			if (autoComplete == null)
				return weatherLocations;
			try {
				foreach (var result in autoComplete.RESULTS) {
					var nameSplitter = result.name.Split(',');
					if (nameSplitter.Length <= 1)
						continue;
					if (!result.type.Equals("city", StringComparison.OrdinalIgnoreCase))
						continue;

					var weatherLocation = new WeatherLocation {
						city = nameSplitter[0].Trim(),
						country = result.c,
						lat = result.lat,
						lon = result.lon
					};

					if (result.c.Equals("US", StringComparison.OrdinalIgnoreCase))
						weatherLocation.state = nameSplitter[1].Trim();

					weatherLocation.Display = (string.IsNullOrWhiteSpace(weatherLocation.state))
						? weatherLocation.city + ", " + weatherLocation.country : weatherLocation.city + ", " + weatherLocation.state;

					weatherLocations.Add(weatherLocation);
				}
				Cacher.Add(cacheKey, weatherLocations);
			} catch (Exception e) {
				Log.Error($"[GetLocations]autocompleteUri={autocompleteUri}|query={query}", e);
			}
			return weatherLocations;
		}

		internal async Task<WeatherCurrent> GetWeatherCurrentFromWeatherUnderground(WeatherLocation location) {
			var cacheKey = "WeatherUndergroundService_GetWeatherCurrentFromWeatherUnderground_" + location.state +
				location.city + location.country;
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (WeatherCurrent)cacheResult;

			var weatherCurrent = new WeatherCurrent();
			var currentObservations = await _weatherUndergroundRequestService.GetCurrentObservation(location.state,
				location.city, location.country);
			if (string.IsNullOrWhiteSpace(currentObservations?.observation_epoch))
				return weatherCurrent;
			try {
				weatherCurrent.epoch = currentObservations.observation_epoch;
				weatherCurrent.description = currentObservations.weather;
				weatherCurrent.dewPoint = currentObservations.dewpoint_f;
				weatherCurrent.humidity = currentObservations.relative_humidity;
				weatherCurrent.precip = currentObservations.precip_today_in + "in";
				weatherCurrent.scale = Scale;
				weatherCurrent.temperature = Convert.ToInt32(currentObservations.temp_f);
				weatherCurrent.weatherIcon = "wi-wu-" + currentObservations.icon;
				weatherCurrent.wind = currentObservations.wind_mph + " " + currentObservations.wind_dir;
				Cacher.Add(cacheKey, weatherCurrent, 5);
			} catch (Exception e) {
				Log.Error("[GetWeatherCurrentFromWeatherUnderground]currentObservations", new List<object> { currentObservations }, e);
			}
			return weatherCurrent;
		}

		internal async Task<List<WeatherDay>> GetDailyHourlyFromWeatherUnderground(WeatherLocation location, int daysInForecast) {
			var cacheKey = "WeatherUndergroundService_GetDailyHourlyFromWeatherUnderground_" + location.state +
							location.city + location.country;
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (List<WeatherDay>)cacheResult;

			var weatherDays = new List<WeatherDay>();
			var hourlyForecasts = await _weatherUndergroundRequestService.GetHourlyForecasts(location.state,
				location.city, location.country);
			if ((hourlyForecasts == null) || (hourlyForecasts.Count <= 0))
				return weatherDays;

			try {
				// Add the hourlies
				var weatherHourliesAll = new List<WeatherHourly>();
				foreach (var hourly in hourlyForecasts) {
					// Add current hour
					int weatherHourlyTemperature;
					int.TryParse(hourly.temp.english, out weatherHourlyTemperature);
					var weatherHourly = new WeatherHourly {
						humidity = hourly.humidity,
						description = hourly.condition,
						precipPercent = hourly.pop + "%",
						scale = Scale,
						temperature = weatherHourlyTemperature,
						epoch = hourly.FCTTIME.epoch,
						weatherIconName = hourly.icon
					};

					int dayNumber;
					int.TryParse(hourly.FCTTIME.mday, out dayNumber);
					weatherHourly.dayNumber = dayNumber;
					weatherHourliesAll.Add(weatherHourly);
				}

				// Break out the hourlies into days
				var hourlies = weatherHourliesAll.GroupBy(w => w.dayNumber);
				foreach (var hourly in hourlies) {
					// Use the highest temp as our day entry
					var hourlySorted = hourly.OrderByDescending(h => h.temperature).First();
					var weatherDay = new WeatherDay {
						weatherHourly = hourly.ToList(),
						epoch = hourlySorted.epoch,
						dayNumber = hourlySorted.dayNumber,
						scale = Scale,
						temperature = hourlySorted.temperature,
						weatherIconName = "wi-wu-" + hourlySorted.weatherIconName
					};
					weatherDays.Add(weatherDay);
					if (weatherDays.Count >= daysInForecast)
						break;
				}

				if (weatherDays.Count >= 1)
					Cacher.Add(cacheKey, weatherDays, 10);
			} catch (Exception e) {
				Log.Error("[GetDailyHourlyFromWeatherUnderground]hourlyForecasts", new List<object> { hourlyForecasts }, e);
			}
			return weatherDays;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherCardApi.Models;
using WeatherCardApi.Provider.ForecastIo.Models;
using WeatherCardApi.Utils;

namespace WeatherCardApi.Provider.ForecastIo.Services {

	public class ForecastIoService : IForecastIoService {
		private readonly IForecastIoRequestService _forecastIoRequestService;
		private const string Scale = "F";

		public ForecastIoService(IForecastIoRequestService forecastIoRequestService) {
			_forecastIoRequestService = forecastIoRequestService;
		}

		/// <summary>
		/// Gets the weather for the passed location using lat/lon. Includes current and forecast weather.
		/// </summary>
		/// <param name="location">WeatherLocation</param>
		/// <param name="daysInForecast">Number of days with 10 being the max</param>
		/// <param name="baseUri">WeatherUnderground base url</param>
		/// <param name="apiKey">WeatherUnderground api key</param>
		/// <returns>Task of WeatherRoot</returns>
		public async Task<WeatherRoot> GetWeather(WeatherLocation location, int daysInForecast, string baseUri, string apiKey) {
			var weatherRoot = new WeatherRoot { weatherLocation = location };
			_forecastIoRequestService.Init(baseUri, apiKey);

			var forecastIoResponse = await GetForecastIoResponse(location.lat, location.lon);
			if (forecastIoResponse == null)
				return weatherRoot;

			weatherRoot.weatherCurrent = GetWeatherCurrentFromForecastIo(forecastIoResponse);
			weatherRoot.weatherDays = GetWeatherDailyHourlyFromForecastIo(forecastIoResponse, daysInForecast);

			return weatherRoot;
		}

		internal async Task<ForecastIoResponse> GetForecastIoResponse(string lat, string lon) {
			return await _forecastIoRequestService.GetForecastIoResponse(lat, lon);
		}

		internal WeatherCurrent GetWeatherCurrentFromForecastIo(ForecastIoResponse forecastIoResponse) {
			var cacheKey = "ForecastIoService_GetWeatherCurrentFromForecastIo_" + forecastIoResponse.Latitude + forecastIoResponse.Longitude;
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (WeatherCurrent)cacheResult;

			var weatherCurrent = new WeatherCurrent();

			try {
				var currently = forecastIoResponse.Currently;
				if (currently.Time <= 0)
					return weatherCurrent;

				weatherCurrent.epoch = currently.Time.ToString();
				weatherCurrent.description = currently.Summary;
				weatherCurrent.dewPoint = Convert.ToInt32(currently.DewPoint);
				weatherCurrent.humidity = (currently.Humidity * 100) + "%";
				weatherCurrent.precip = "n/a";
				weatherCurrent.scale = Scale;
				weatherCurrent.temperature = Convert.ToInt32(currently.Temperature);
				weatherCurrent.weatherIcon = "wi-forecast-io-" + currently.Icon;
				weatherCurrent.wind = (Convert.ToInt32(currently.WindSpeed) <= 0) ? "0" : currently.WindSpeed + " " + DegreesToCardinal(currently.WindBearing);
				Cacher.Add(cacheKey, weatherCurrent, 5);
			} catch (Exception e) {
				Log.Error("[GetWeatherCurrentFromForecastIo]forecastIoResponse.Currently",
					new List<object> { forecastIoResponse.Currently }, e);
			}
			return weatherCurrent;
		}

		internal List<WeatherDay> GetWeatherDailyHourlyFromForecastIo(ForecastIoResponse forecastIoResponse, int daysInForecast) {
			var cacheKey = "ForecastIoService_GetWeatherDailyHourlyFromForecastIo_" + forecastIoResponse.Latitude + forecastIoResponse.Longitude;
			var cacheResult = Cacher.Get(cacheKey);
			if (cacheResult != null)
				return (List<WeatherDay>)cacheResult;

			var weatherDays = new List<WeatherDay>();
			try {
				// Add days
				foreach (var daily in forecastIoResponse.Daily.Data) {
					if (daily.Time <= 0)
						continue;

					var weatherDay = new WeatherDay {
						epoch = daily.Time.ToString(),
						dayNumber = UnixTimeStampToDateTime(daily.Time).Day,
						scale = Scale,
						temperature = Convert.ToInt32(daily.TemperatureMax),
						weatherIconName = "wi-forecast-io-" + daily.Icon
					};
					DateTime dailyDateTime;
					DateTime.TryParse(daily.Time.ToString(), out dailyDateTime);
					weatherDays.Add(weatherDay);
					if (weatherDays.Count >= daysInForecast)
						break;
				}

				// Add hourlies
				var dayNumber = 0;
				var weatherHourlies = new List<WeatherHourly>();
				foreach (var hourly in forecastIoResponse.Hourly.Data) {
					if (hourly.Time <= 0)
						continue;

					var hourlyDay = UnixTimeStampToDateTime(hourly.Time).Day;

					// Add hourlies to the day
					if (dayNumber != hourlyDay) {
						var weatherDay = weatherDays.FirstOrDefault(d => d.dayNumber == dayNumber);
						if (weatherDay != null) {
							weatherDay.weatherHourly = weatherHourlies;
						}
						weatherHourlies = new List<WeatherHourly>();
					}

					var weatherHourly = new WeatherHourly {
						description = hourly.Summary,
						epoch = hourly.Time.ToString(),
						humidity = (hourly.Humidity * 100) + "%",
						precipPercent = (hourly.PrecipProbability * 100) + "%",
						scale = Scale,
						temperature = Convert.ToInt32(hourly.Temperature)
					};
					weatherHourlies.Add(weatherHourly);

					dayNumber = hourlyDay;
				}
				Cacher.Add(cacheKey, weatherDays, 5);
			} catch (Exception e) {
				Log.Error("[GetWeatherDailyHourlyFromForecastIo]forecastIoResponse.Daily|forecastIoResponse.Hourly",
					new List<object> { forecastIoResponse.Daily, forecastIoResponse.Hourly }, e);
			}
			return weatherDays;
		}

		private static string DegreesToCardinal(double degrees) {
			string[] caridnals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
			return caridnals[(int)Math.Round((degrees % 360) / 45)];
		}

		private static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}
	}
}
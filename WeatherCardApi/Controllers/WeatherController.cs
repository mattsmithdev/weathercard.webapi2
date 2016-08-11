using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WeatherCardApi.Models;
using WeatherCardApi.Services;

namespace WeatherCardApi.Controllers {

	public class WeatherController : ApiController {
		private readonly IWeatherService _iWeatherService;

		public WeatherController(IWeatherService weatherService) {
			_iWeatherService = weatherService;
		}

		// GET: api/Weather/provider
		public async Task<WeatherRoot> Get(string id) {
			//return new WeatherRoot();

			var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
			var location = new WeatherLocation {
				city = query["city"],
				state = query["state"],
				country = query["country"],
				lat = query["lat"],
				lon = query["lon"]
			};
			if ((string.IsNullOrWhiteSpace(location.city)) || (location.city.Equals("undefined", StringComparison.OrdinalIgnoreCase)))
				return new WeatherRoot();
			return await _iWeatherService.GetWeather(location, id);
		}
	}
}
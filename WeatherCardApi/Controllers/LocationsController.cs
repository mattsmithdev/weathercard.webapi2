using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WeatherCardApi.Models;
using WeatherCardApi.Services;

namespace WeatherCardApi.Controllers {

	public class LocationsController : ApiController {
		private readonly IWeatherService _iWeatherService;

		public LocationsController(IWeatherService weatherService) {
			_iWeatherService = weatherService;
		}

		// GET: api/Locations/query
		public async Task<List<WeatherLocation>> Get(string id) {
			return await _iWeatherService.GetLocations(id);
		}

		// GET: api/Locations/
		public async Task<WeatherLocation> Get() {
			var clientIp = GetClientIpAddress(Request);
			if (string.IsNullOrWhiteSpace(clientIp))
				return new WeatherLocation();

			return await _iWeatherService.GetLocation(clientIp);
		}

		private static string GetClientIpAddress(HttpRequestMessage request) {
			//return "74.103.166.10";

			const string httpContext = "MS_HttpContext";
			const string remoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

			if (request.Properties.ContainsKey(httpContext)) {
				dynamic ctx = request.Properties[httpContext];
				if (ctx != null) {
					return ctx.Request.UserHostAddress;
				}
			}

			if (!request.Properties.ContainsKey(remoteEndpointMessage))
				return null;
			dynamic remoteEndpoint = request.Properties[remoteEndpointMessage];
			return remoteEndpoint != null ? remoteEndpoint.Address : null;
		}
	}
}
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using WeatherCardApi.Filters;

namespace WeatherCardApi {

	public static class WebApiConfig {

		public static void Register(HttpConfiguration config) {
			// Web API configuration and services

			EnableCrossSiteRequests(config);

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

			//config.Filters.Add(new LocalRequestOnlyAttribute());
		}

		private static void EnableCrossSiteRequests(HttpConfiguration config) {
			var cors = new EnableCorsAttribute(
				origins: "*",
				headers: "*",
				methods: "*");
			config.EnableCors(cors);
		}
	}
}
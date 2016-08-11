using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;
using WeatherCardApi.Provider.ForecastIo.Services;
using WeatherCardApi.Provider.WeatherUnderground.Services;
using WeatherCardApi.Services;

namespace WeatherCardApi {

	public class WebApiApplication : System.Web.HttpApplication {

		protected void Application_Start() {
			var container = new Container();
			container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
			container.Register<IWeatherUndergroundRequestService, WeatherUndergroundRequestService>(Lifestyle.Scoped);
			container.Register<IWeatherUndergroundService, WeatherUndergroundService>(Lifestyle.Scoped);
			container.Register<IForecastIoService, ForecastIoService>(Lifestyle.Scoped);
			container.Register<IForecastIoRequestService, ForecastIoRequestService>(Lifestyle.Scoped);
			container.Register<IWeatherService, WeatherService>(Lifestyle.Scoped);
			container.Verify();

			GlobalConfiguration.Configuration.DependencyResolver =
		new SimpleInjectorWebApiDependencyResolver(container);

			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}
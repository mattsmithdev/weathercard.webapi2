using System.Threading.Tasks;
using WeatherCardApi.Provider.ForecastIo.Models;

namespace WeatherCardApi.Provider.ForecastIo.Services {

	public interface IForecastIoRequestService {

		void Init(string baseUri, string apiKey);

		Task<ForecastIoResponse> GetForecastIoResponse(string lat, string lon);
	}
}
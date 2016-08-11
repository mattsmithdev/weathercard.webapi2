using System.Collections.Generic;

namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class Hourly {
		public string Summary { get; set; }
		public string Icon { get; set; }
		public List<HourForecast> Data { get; set; }
	}
}
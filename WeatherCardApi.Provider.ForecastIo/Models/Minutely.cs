using System.Collections.Generic;

namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class Minutely {
		public string Summary { get; set; }
		public string Icon { get; set; }
		public List<MinuteForecast> Data { get; set; }
	}
}
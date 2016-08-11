using System.Collections.Generic;

namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class Daily {
		public string Summary { get; set; }
		public string Icon { get; set; }
		public List<DailyForecast> Data { get; set; }
	}
}
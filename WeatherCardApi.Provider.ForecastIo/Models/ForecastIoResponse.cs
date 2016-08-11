using System.Collections.Generic;

namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class ForecastIoResponse {
		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string Timezone { get; set; }
		public float Offset { get; set; }
		public Currently Currently { get; set; }
		public Minutely Minutely { get; set; }
		public Hourly Hourly { get; set; }
		public Daily Daily { get; set; }
		public List<Alert> Alerts { get; set; }
		public Flags Flags { get; set; }
		public int NumberOfCalls { get; set; }
	}
}
using System.Collections.Generic;

namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class Flags {
		public List<string> Sources { get; set; }
		public List<string> IsdStations { get; set; }
		public List<string> LampStations { get; set; }
		public List<string> MetarStations { get; set; }
		public List<string> DarkskyStations { get; set; }
		public string Units { get; set; }
	}
}
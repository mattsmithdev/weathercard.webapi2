using System.Collections.Generic;

namespace WeatherCardApi.Provider.WeatherUnderground.Models.AutoComplete {

	public class RESULT {
		public string name { get; set; }
		public string type { get; set; }
		public string c { get; set; }
		public string zmw { get; set; }
		public string tz { get; set; }
		public string tzs { get; set; }
		public string l { get; set; }
		public string ll { get; set; }
		public string lat { get; set; }
		public string lon { get; set; }
	}

	public class AutoComplete {
		public List<RESULT> RESULTS { get; set; }
	}
}
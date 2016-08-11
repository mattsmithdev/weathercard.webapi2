using System.Collections.Generic;

namespace WeatherCardApi.Models {

	public class WeatherLocation {
		public string Display { get; set; }
		public string country { get; set; }
		public string state { get; set; }
		public string city { get; set; }
		public string lat { get; set; }
		public string lon { get; set; }
	}

	public class WeatherCurrent {
		public int temperature { get; set; }
		public string weatherIcon { get; set; }
		public string description { get; set; }
		public string scale { get; set; }
		public string epoch { get; set; }
		public string humidity { get; set; }
		public int dewPoint { get; set; }
		public string wind { get; set; }
		public string precip { get; set; }
	}

	public class WeatherHourly {
		public string epoch { get; set; }
		public int temperature { get; set; }
		public string scale { get; set; }
		public string humidity { get; set; }
		public string description { get; set; }
		public string precipPercent { get; set; }
		public int dayNumber { get; set; }
		public string weatherIconName { get; set; }
	}

	public class WeatherDay {

		public WeatherDay() {
			weatherHourly = new List<WeatherHourly>();
		}

		public string epoch { get; set; }
		public int dayNumber { get; set; }
		public string dayName { get; set; }
		public string weatherIconName { get; set; }
		public int temperature { get; set; }
		public string scale { get; set; }
		public List<WeatherHourly> weatherHourly { get; set; }
	}

	public class WeatherRoot {

		public WeatherRoot() {
			weatherLocation = new WeatherLocation();
			weatherCurrent = new WeatherCurrent();
			weatherDays = new List<WeatherDay>();
		}

		public WeatherLocation weatherLocation { get; set; }
		public WeatherCurrent weatherCurrent { get; set; }
		public List<WeatherDay> weatherDays { get; set; }
	}
}
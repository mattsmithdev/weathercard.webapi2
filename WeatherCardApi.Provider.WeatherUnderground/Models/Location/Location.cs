namespace WeatherCardApi.Provider.WeatherUnderground.Models.Location {

	public class Features {
		public int geolookup { get; set; }
	}

	public class Response {
		public string version { get; set; }
		public string termsofService { get; set; }
		public Features features { get; set; }
	}

	public class LocationRoot {
		public Response response { get; set; }
		public Location location { get; set; }
	}

	public class Location {
		public string type { get; set; }
		public string country { get; set; }
		public string country_iso3166 { get; set; }
		public string country_name { get; set; }
		public string state { get; set; }
		public string city { get; set; }
		public string tz_short { get; set; }
		public string tz_long { get; set; }
		public string lat { get; set; }
		public string lon { get; set; }
		public string zip { get; set; }
		public string magic { get; set; }
		public string wmo { get; set; }
		public string l { get; set; }
		public string requesturl { get; set; }
		public string wuiurl { get; set; }
	}
}
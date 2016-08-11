namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class Currently {
		public long Time { get; set; }
		public string Summary { get; set; }
		public string Icon { get; set; }
		public float NearestStormDistance { get; set; }
		public float NearestStormBearing { get; set; }
		public float PrecipIntensity { get; set; }
		public float PrecipProbability { get; set; }
		public string PrecipType { get; set; }
		public float Temperature { get; set; }
		public float ApparentTemperature { get; set; }
		public float DewPoint { get; set; }
		public float WindSpeed { get; set; }
		public float WindBearing { get; set; }
		public float CloudCover { get; set; }
		public float Humidity { get; set; }
		public float Pressure { get; set; }
		public float Visibility { get; set; }
		public float Ozone { get; set; }
	}
}
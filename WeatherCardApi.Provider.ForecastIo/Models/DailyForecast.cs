namespace WeatherCardApi.Provider.ForecastIo.Models {

	public class DailyForecast {
		public long Time { get; set; }
		public string Summary { get; set; }
		public string Icon { get; set; }
		public long SunriseTime { get; set; }
		public long SunsetTime { get; set; }
		public float MoonPhase { get; set; }
		public float PrecipAccumulation { get; set; }
		public float PrecipIntensity { get; set; }
		public float PrecipIntensityMax { get; set; }
		public long PrecipIntensityMaxTime { get; set; }
		public float PrecipProbability { get; set; }
		public string PrecipType { get; set; }
		public float TemperatureMin { get; set; }
		public long TemperatureMinTime { get; set; }
		public float TemperatureMax { get; set; }
		public long TemperatureMaxTime { get; set; }
		public float ApparentTemperatureMin { get; set; }
		public long ApparentTemperatureMinTime { get; set; }
		public float ApparentTemperatureMax { get; set; }
		public long ApparentTemperatureMaxTime { get; set; }
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
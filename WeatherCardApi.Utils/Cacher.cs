using System;

namespace WeatherCardApi.Utils {

	public static class Cacher {

		public static object Get(string key) {
			return NomadMemCache.Cacher.Instance.GetValue<object>(key);
		}

		public static void Add(string key, object value, int expirationInMinutes = 0) {
			NomadMemCache.Cacher.Instance.Add(key, value,
				expirationInMinutes > 0 ? DateTimeOffset.Now.AddMinutes(expirationInMinutes) : DateTimeOffset.MaxValue);
		}

		public static void Remove(string key) {
			NomadMemCache.Cacher.Instance.Delete(key);
		}
	}
}
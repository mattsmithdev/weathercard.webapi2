using MemoryCache;

namespace WeatherCardApi.Provider.Util {

	public static class Cacher {

		public static object Get(string key) {
			return Exists(key) ? Cache.Get(key) : null;
		}

		public static void Add(string key, object value, int expirationInMinutes = 0) {
			if (expirationInMinutes > 0) {
				Cache.Store(key, value, expirationInMinutes * 60000);
			} else {
				Cache.Store(key, value);
			}
		}

		public static bool Exists(string key) {
			return Cache.Exists(key);
		}

		public static void Remove(string key) {
			if (Exists(key))
				Cache.Remove(key);
		}
	}
}
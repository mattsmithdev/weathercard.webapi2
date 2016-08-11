using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WeatherCardApi.Utils {

	public static class RestClient {

		/// <summary>
		/// Retrieve json data. Supports jsonp.
		/// </summary>
		/// <typeparam name="T">Type of object to deserialize into</typeparam>
		/// <param name="baseUri">Destination base uri (ex, http://www.google.com/)</param>
		/// <param name="requestPath">Destination uri path (ex, query/keyword)</param>
		/// <returns>Instance of Object on success or default Object on failure</returns>
		public static async Task<T> Get<T>(string baseUri, string requestPath) where T : new() {
			var handler = new HttpClientHandler() {
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			using (var client = new HttpClient(handler)) {
				client.BaseAddress = new Uri(baseUri);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/javascript"));

				try {
					var response = await client.GetAsync(requestPath);
					response.EnsureSuccessStatusCode();
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(content));
				} catch (HttpRequestException e) {
					Log.Error($"[Get]baseUri={baseUri}|requestPath={requestPath}", e);
					return default(T);
				}
			}
		}
	}
}
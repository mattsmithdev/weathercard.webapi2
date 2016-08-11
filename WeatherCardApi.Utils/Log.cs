using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;

namespace WeatherCardApi.Utils {

	/// <summary>
	/// File logger
	/// </summary>
	public static class Log {
		private static readonly Logger Logger;

		static Log() {
			var config = new LoggingConfiguration();

			var fileTarget = new FileTarget();
			config.AddTarget("file", fileTarget);

			fileTarget.FileName = "${basedir}/weathercardapi.log";
			fileTarget.Layout = @"${date: format = HH\:mm\:ss} ${logger} ${appdomain:format={1\}} ${level} ${message}";
			var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
			config.LoggingRules.Add(rule);

			LogManager.Configuration = config;

			Logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// Debug message logger
		/// </summary>
		/// <param name="message">message</param>
		public static void Debug(string message) {
			Logger.Debug(message);
		}

		/// <summary>
		/// Debug message logger
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="objects">list of objects to encode and include</param>
		public static void Debug(string message, List<object> objects) {
			Logger.Debug(message + EncodeObjects(objects));
		}

		/// <summary>
		/// Error message logger
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="e">(optional) Exception to include</param>
		public static void Error(string message, Exception e = null) {
			if (e != null)
				message += ":" + e;
			Logger.Error(message);
		}

		/// <summary>
		/// Error message logger
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="objects">list of objects to encode and include</param>
		/// <param name="e">(optional) Exception to include</param>
		public static void Error(string message, List<object> objects, Exception e = null) {
			message = message + EncodeObjects(objects);
			if (e != null)
				message += ":" + e;
			Logger.Debug(message);
		}

		/// <summary>
		/// Info message logger
		/// </summary>
		/// <param name="message">message</param>
		public static void Info(string message) {
			Logger.Info(message);
		}

		/// <summary>
		/// Info message logger
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="objects">list of objects to encode and include</param>
		public static void Info(string message, List<object> objects) {
			Logger.Debug(message + EncodeObjects(objects));
		}

		private static string EncodeObjects(IReadOnlyCollection<object> objects) {
			var encoded = "";
			if ((objects == null) || (objects.Count <= 0))
				return encoded;
			foreach (var o in objects) {
				encoded += "|" + JsonConvert.SerializeObject(o);
			}
			return encoded;
		}
	}
}
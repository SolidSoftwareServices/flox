using System;
using EI.RP.CoreServices.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EI.RP.CoreServices.Serialization
{
	public static class JsonExtensions
	{
		static JsonExtensions()
		{ }

		private static readonly Lazy<JsonSerializerSettings> SerializerSettings = new Lazy<JsonSerializerSettings>(() =>
		{
			var serializerSettings = new JsonSerializerSettings();
			serializerSettings.NullValueHandling = NullValueHandling.Ignore;

#if DEBUG
			serializerSettings.Formatting = Formatting.Indented;
			//  serializerSettings.TraceWriter = new NLogTraceWriter();
#endif
			return serializerSettings;
		});

		private static readonly Lazy<JsonSerializerSettings> CamelSerializerSettings = new Lazy<JsonSerializerSettings>(
			() =>
			{
				var serializerSettings = new JsonSerializerSettings();
				serializerSettings.NullValueHandling = NullValueHandling.Ignore;
				serializerSettings.ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				};
#if DEBUG
				serializerSettings.Formatting = Formatting.Indented;
				//  serializerSettings.TraceWriter = new NLogTraceWriter();
#endif
				return serializerSettings;
			});

		private static readonly Lazy<JsonSerializerSettings> FullSerializerSettings = new Lazy<JsonSerializerSettings>(
			() =>
			{
				var settings = new JsonSerializerSettings();
				settings.NullValueHandling = NullValueHandling.Ignore;
				settings.TypeNameHandling = TypeNameHandling.Auto;

#if DEBUG
				settings.Formatting = Formatting.Indented;
				//settings.TraceWriter=new NLogTraceWriter();
#endif
				return settings;
			});

		//TODO: RE-ENGINEER
		public static string ToJson<TSource>(this TSource src, bool serializeTypeNames = false, bool camelCase = false)
		{
			var json = JsonConvert.SerializeObject(src
				, serializeTypeNames
					? FullSerializerSettings.Value
					: camelCase
						? CamelSerializerSettings.Value
						: SerializerSettings.Value
			);

			return json;
		}

		public static TObject JsonToObject<TObject>(this string jsonSerializedObject, bool containsTypeNames = false)
		{
			if (jsonSerializedObject == null) return default(TObject);

			var jsonToObject = JsonConvert.DeserializeObject<TObject>(jsonSerializedObject,
				containsTypeNames ? FullSerializerSettings.Value : SerializerSettings.Value);
			return jsonToObject;
		}

		public static string JsonPrettyPrint(this string jsonSerializedObject, bool containsTypeNames = false)
		{
			if (jsonSerializedObject == null) return string.Empty;

			var jsonPrettyPrint = jsonSerializedObject.JsonToObject<dynamic>();

			return jsonPrettyPrint.ToJson();
		}
	}
}
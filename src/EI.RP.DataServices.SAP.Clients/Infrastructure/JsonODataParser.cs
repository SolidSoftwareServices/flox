using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EI.RP.DataServices.SAP.Clients.Infrastructure
{

	/// <summary>
	/// all this hac is because our sap only accept deep objects for some operations but the version of odata exposed(v2) does not accept them in the protocol so we have to bypass them
	/// </summary>
	internal static class JsonODataParser
	{
		public static TDto ODataJsonResultToObject<TDto>(this string obj)
		{
			var src = JObject.Parse(obj);

			var result=ParseObject(src);

			var json = JsonConvert.SerializeObject(result);
			return json.JsonToObject<TDto>();
		}

		private static readonly string[] MetaKeys =
		{
			"__metadata",
			"__deferred"
		};
		private static JObject ParseObject(JObject src)
		{
			var target = new JObject();

			foreach (var prop in src)
			{
				if (MetaKeys.Contains(prop.Key)) continue;
				

				target.Add(prop.Key, Parse(prop.Value));
			}

			if (!target.HasValues)
			{
				return null;
			}
			return target;
		}

		private static JToken Parse(JToken propValue)
		{
			switch (propValue.Type)
			{
				case JTokenType.Object:

					var jObject = (JObject)propValue;

					if (jObject.ContainsKey("results"))
					{
						return ParseArray((JArray)jObject["results"]);
					}
					return ParseObject(jObject);
				

				case JTokenType.Raw:
				case JTokenType.Constructor:
				case JTokenType.Property:
				case JTokenType.Comment:
				case JTokenType.Undefined:
				case JTokenType.Integer:
				case JTokenType.Float:
				case JTokenType.String:
				case JTokenType.Boolean:
				case JTokenType.Null:
				case JTokenType.Date:
				case JTokenType.Bytes:
				case JTokenType.Guid:
				case JTokenType.Uri:
				case JTokenType.TimeSpan:
				case JTokenType.None:
				case JTokenType.Array:
					return propValue;
				default:
					throw new ArgumentOutOfRangeException(nameof(propValue),"not supported");
			}
		}

		private static JToken ParseArray(JArray jArray)
		{
			var result = new JArray();
			foreach (var item in jArray)
			{
				result.Add(ParseObject((JObject)item));
			}

			return result;
		}
	}
}
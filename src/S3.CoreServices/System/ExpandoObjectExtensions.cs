using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using S3.CoreServices.System.FastReflection;

namespace S3.CoreServices.System
{
	public static class ExpandoObjectExtensions
	{
		public static string AsString(this ExpandoObject src)
		{
			return string.Join(" - ", src.Select(x => $"{x.Key}={x.Value}"));
		}

		public static dynamic ToDynamic<T>(this T value,params string[] discardPropertyNames)
		{
			return ToExpandoObject(value, discardPropertyNames);
		}
		public static dynamic ToDynamic<T>(this T value, bool removeNullProperties)
		{
			var expandoObject = (IDictionary<string,object>)ToExpandoObject(value);
			if (removeNullProperties)
			{
				expandoObject= expandoObject.Where(x=>x.Value!=null).ToDictionary(x=>x.Key,x=>x.Value);
			}
			return expandoObject;
		}
		public static ExpandoObject ToExpandoObject(this object obj, params string[] discardPropertyNames)
		{
			ExpandoObject result;
			if (obj == null)
			{
				result = null;
			}
			else if (obj is ExpandoObject)
			{
				result=(ExpandoObject) obj;
			}
			else if (obj is Dictionary<string, object>)
			{
				result= FromDictionary();
			}
			else
			{
				result = Default();
			}

			return result;

			ExpandoObject FromDictionary()
			{
				IDictionary<string, object> expando = new ExpandoObject();

				var dictionary = (Dictionary<string, object>)obj;
				foreach (var key in dictionary.Keys.Where(x=>!discardPropertyNames.Contains(x)))
				{
					expando.Add(key, dictionary[key]);
				}

				return (ExpandoObject) expando;
			}

			ExpandoObject Default()
			{
				IDictionary<string, object> expando = new ExpandoObject();

				foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
				{
					if (!discardPropertyNames.Contains(property.Name))
					{
						expando.Add(property.Name, property.GetValue(obj));
					}
				}

				return (ExpandoObject) expando;
			}
		}

		public static TValue GetValueOrNull<TValue>(this ExpandoObject src, string optionalPropertyName)
			where TValue : class
		{
			var d = (IDictionary<string, object>) src;
			if (!d.ContainsKey(optionalPropertyName)) return null;

			return (TValue) d[optionalPropertyName];
		}

		public static TValue ToObjectOfType<TValue>(this ExpandoObject src,bool failIfPropertyNameNotFound=true) where TValue:new()
		{
			return (TValue) src.ToObjectOfType(typeof(TValue), failIfPropertyNameNotFound);
		}
		public static object ToObjectOfType(this ExpandoObject src,Type type, bool failIfPropertyNameNotFound=true) 
		{
			var result = ObjectBuilder.Default.Build(type);
			if (src != null)
			{
				var dictionary = (IDictionary<string, object>) src;

				foreach (var key in dictionary.Keys)
				{
					result.SetPropertyValueFast(key, dictionary[key],failIfPropertyNameNotFound);
				}
			}

			return result;

		}
		public static object WithoutProperties(this ExpandoObject src, IEnumerable<string> propertyNames)
		{
			IDictionary<string, object> expando = src;
			foreach (var propertyName in propertyNames)
			{
				if (expando.ContainsKey(propertyName))
				{
					expando.Remove(propertyName);
				}
			}

			return expando;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System.FastReflection;
using Force.DeepCloner;
using NLog;

namespace EI.RP.CoreServices.System
{
	public static class ObjectExtensions
	{
		public static byte[] ToByteArray<TObject>(this TObject obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static TObject[] ToOneItemArray<TObject>(this TObject src)
		{
			return new[] {src};
		}
		public static List<TObject> ToOneItemList<TObject>(this TObject src)
		{
			return new List<TObject> {src};
		}
		public static TObject ToObject<TObject>(this byte[] arrBytes)
		{
			using (var memStream = new MemoryStream())
			{
				var binForm = new BinaryFormatter();
				memStream.Write(arrBytes, 0, arrBytes.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = binForm.Deserialize(memStream);
				return (TObject) obj;
			}
		}


		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		public static TObject CloneDeep<TObject>(this TObject src) where TObject : class
		{
			try
			{
				return src?.DeepClone();
			}
			catch (ArgumentException ex)
			{
				Logger.Warn(()=>$"{src.ToJson(serializeTypeNames:true)}, {ex}");
				throw;
			}
		}



		public static T UpdateWithChangesFrom<T>(this T target, T source, Action<T, T> adjustmentsAfter = null)
			where T : class
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			//TODO: [MM] improve perfomance
			var result = target.CloneDeep();
			var properties = target.GetType().GetPropertiesFast().Union(typeof(T).GetPropertiesFast())
				.Where(prop => prop.CanRead && prop.CanWrite);

			foreach (var prop in properties)
			{
				var candidateValue = prop.GetValue(source, null);
				var actualValue = prop.GetValue(result, null);
				if (candidateValue != null)
				{
					var isEnumerable = !(candidateValue is string) && candidateValue.GetType()
						                   .GetInterfaces()
						                   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() ==
						                             typeof(IEnumerable<>));
					if (isEnumerable && ((IEnumerable) candidateValue).Cast<object>().Any())
					{
						result.SetPropertyValueFast(prop, candidateValue);
					}
					else if (!isEnumerable && !candidateValue.Equals(actualValue))
					{
						result.SetPropertyValueFast(prop, candidateValue);
					}
				}
			}

			adjustmentsAfter?.Invoke(source, result);
			return result;
		}

		public static object MergeObjects(this object source, params object[] withObjects)
		{
			return source.MergeObjects(false, withObjects);
		}
		public static object MergeObjects(this object source, bool preserveLast,  params object[] withObjects)
		{
			var merged = (IDictionary<string, object>) source.ToExpandoObject();
			for (var i = 0; i < withObjects.Length; i++)
			{
				var withObject = withObjects[i];
				var expandoObject = withObject as ExpandoObject ?? withObject.ToExpandoObject();

				var current = (IDictionary<string, object>) (expandoObject );
				foreach (var key in current.Keys)
				{
					var targetKey = merged.Keys.SingleOrDefault(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase));
					var srcValue = current[key];
					if (targetKey == null)
					{
						merged.Add(key, current[key]);
					}
					else if (merged[targetKey] == null && srcValue != null)
					{
						merged[targetKey] = srcValue;
					}
					else if (srcValue!=null && merged[targetKey] != srcValue)
					{
						if (preserveLast)
						{
							merged[targetKey] = srcValue;
						}
						else
						{
							throw new InvalidOperationException("cannot merge 2 different values");
						}
					}
				}

			}

			return merged;
		}

        public static bool IsOneOf<T>(this T self, params T[] values) => values.Contains(self);
	}
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using Newtonsoft.Json.Linq;

namespace EI.RP.CoreServices.Caching.Models
{
	static class CachedItemSerializationExtensions
	{
		public static async Task<byte[]> CacheSerializeAsync(this CachedItem item,IEncryptionService encryptionService=null)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			var type = item.Item?.GetType();
			if (type != null)
			{
				var itemType = ResolveSerializationItemType(type);

				item.ItemType = itemType;
			}

			var json = item.ToJson(true);
			var data = await (encryptionService?.EncryptAsync(json)??Task.FromResult(json));
			return data.ToByteArray();
		}
		

		public static async Task<byte[]> CacheSerializeAsync<TObject>(this TObject item,IEncryptionService encryptionService=null)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));
			return await new CachedItem {Item = item}.CacheSerializeAsync(encryptionService);
		}

		public static async Task<CachedItem> CacheDeserializeAsync(this byte[] bytes,IEncryptionService encryptionService=null)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));
			var source = bytes.ToObject<string>();
			var json = await (encryptionService?.DecryptAsync(source)??Task.FromResult(source));

			var cachedItem = json.JsonToObject<CachedItem>(containsTypeNames:true);
			if (cachedItem.Item != null)
			{
				var objectType = TypesFinder.Resolver.FindType(cachedItem.ItemType, false);
				if (objectType == null)
				{
					cachedItem = null;
				}
				else
				{
					if (cachedItem.Item is JObject jObject)
					{
						cachedItem.Item = jObject.ToObject(objectType);
					}
					else
					{
						cachedItem.Item = cachedItem.Item is JArray
							? ((JArray) cachedItem.Item)?.ToObject(objectType)
							: cachedItem.Item;
					}
				}
			}

			return cachedItem;
		}
		private static readonly ConcurrentDictionary<Type,string> CachedSerializationItemTypes=new ConcurrentDictionary<Type, string>();
		private static string ResolveSerializationItemType(Type type)
		{
			return CachedSerializationItemTypes.GetOrAdd(type, (t) =>
			{
				string itemType;
				if (t!= typeof(string) && t.ImplementsOpenGeneric(typeof(IEnumerable<>)))
				{
					if (t.IsArray)
					{
						itemType = t.GetInterfaces().Single(x =>
							x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).FullName;
					}
					else
					{
						itemType = typeof(IEnumerable<>).MakeGenericType(t.GenericTypeArguments).FullName;
					}
				}
				else
				{
					itemType = t.FullName;
				}

				if (TypesFinder.Resolver.FindType(itemType, false) == null)
				{
					throw new InvalidOperationException("Could not find the type used to serialize this item");
				}
				
				return itemType;
			});
		}
	
	}
}
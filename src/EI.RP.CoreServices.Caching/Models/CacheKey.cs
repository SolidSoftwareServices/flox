using System;
using System.Linq;
using System.Reflection;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace EI.RP.CoreServices.Caching.Models
{
	class CacheScope:TypedStringValue<CacheScope>
	{	
		[JsonConstructor]
		private CacheScope()
		{
		}
		private CacheScope(string value,string debuggerName=null ) : base(value, disableVerifyValueExists:true,debuggerFriendlyDisplayValue:debuggerName)
		{
		}

		public const string NoScope = null;

		/// <summary>
		/// The key is not contextualised by scope
		/// </summary>
		public static readonly CacheScope GlobalScope  = new CacheScope("_g_",nameof(GlobalScope));
	}

	internal class CacheKeyScope
	{


		internal const string CachePrefix = "_<RP>_";
		protected const string Separator = "||";
		
		public static string StageName { get; set; }

		public static string VersionKeyPart { get; }= $"v={Assembly.GetEntryAssembly().GetName().Version}";

		public static string EnvironmentKeyPart() => $"e={StageName}";

		public CacheKeyScope(string scope):this((CacheScope)scope){}
		public CacheKeyScope(CacheScope scope)
		{
			Scope = scope;
			if (Scope is null || Scope==CacheScope.NoScope)
			{
				Scope = CacheScope.GlobalScope;
			}
		}

		
		public CacheScope Scope { get; }
		
		public override string ToString()
		{
			return $"{CachePrefix}{Separator}s={Scope?.ToString()?.ToLowerInvariant()}{Separator}{EnvironmentKeyPart()}{Separator}{VersionKeyPart}{Separator}";
		}

		public static implicit operator string(CacheKeyScope src)
		{
			return src?.ToString();
		}
	}

	internal class CacheKey<TKey,TValue> : CacheKeyScope
	{
		public CacheKey(TKey key) : this(CacheScope.GlobalScope,key)
		{
		}
		public CacheKey(string scope, TKey key) : this((CacheScope)scope,key)
		{
		}
		public CacheKey(CacheScope scope, TKey key):base(scope)
		{
			Key = key != null ? key : throw new ArgumentNullException(nameof(key));
		}

		public TKey Key { get;}
		const string keyToken = "k";
		public override string ToString()
		{
			
			return $"{base.ToString()}t={Key.GetType().FullName}{Separator}{keyToken}={Key.ToJson(true)}{Separator}tv={typeof(TValue).FullName}{Separator}";
		}

		public static implicit operator string(CacheKey<TKey,TValue> src)
		{
			return src?.ToString();
		}

		public static string GetUserKey(string keyStr)
		{
			if (keyStr == null) throw new ArgumentNullException(nameof(keyStr));
			var key = keyStr.Split(new[] {Separator}, StringSplitOptions.None).Select(x => x.Split('='))
				.SingleOrDefault(x => x[0] == keyToken)?[1];
			if(key==null) throw new ArgumentException("not a valid key string");
			return key;
		}
	}


	public static class CacheExtensions
	{
		public static bool IsCacheKey(this string value)
		{
			return value?.StartsWith(CacheKeyScope.CachePrefix)??false;
		}
	}
	
}
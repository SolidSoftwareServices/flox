using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Platform;
using Ei.Rp.Mvc.Core;
using Ei.Rp.Mvc.Core.Cryptography.Urls;

namespace EI.RP.NavigationPrototype.Infrastructure
{
	class AppSettings : IEncryptionSettings,
		IAppCookieSettings,
		IUrlEncryptionSettings,
		IPlatformSettings,
		IRequestPipelineCoreSettings
	{
	
		public IEnumerable<string> AppCookieNames { get; }=new string[0];
		public bool EncryptUrls { get; } = false;
		public Task<string> EncryptionPassPhraseAsync()
		{
			return Task.FromResult("877$$%578657");
		}

		public Task<string> EncryptionSaltValueAsync()
		{
			return Task.FromResult(")(00(0%%");
		}

		public Task<string> EncryptionInitVectorAsync()
		{
			return Task.FromResult("€545ss££22+=");
		}

		
		public bool HealthChecksEnabled { get; }
		public bool ProfileInDetail { get; }
		
		public bool IsCacheEnabled { get; }
		public bool IsCachePreLoaderEnabled { get; }
		public CacheProviderType CacheProviderType { get; }
		public TimeSpan ExpireCacheItemsWhenNotUsedFor { get; }
		public Task<string> RedisConnectionString()
		{
			throw new NotImplementedException();
		}

		public bool IsRequestVerboseLoggingEnabled => false;
	}
}
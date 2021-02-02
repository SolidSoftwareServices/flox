using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using S3.CoreServices.Encryption;
using S3.CoreServices.Platform;
using S3.Mvc.Core;
using S3.Mvc.Core.Cryptography.Urls;

namespace S3.App.AspNetCore3_1.Infrastructure
{
	class AppSettings : IEncryptionSettings,
		IUrlEncryptionSettings,
		IPlatformSettings,
		IRequestPipelineCoreSettings
	{
	
		public bool EncryptUrls { get; } = true;
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

		
		public bool ProfileInDetail { get; }

		public bool IsDeferredComponentLoadEnabled { get; } = true;
		public string DeferredComponentLoaderViewEmbeddedResourceName { get; } = null;
		public bool IsProfilerEnabled { get; } = true;

		

		public bool IsRequestVerboseLoggingEnabled => false;
	}
}
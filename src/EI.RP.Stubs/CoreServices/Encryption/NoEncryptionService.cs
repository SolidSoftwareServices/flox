using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Encryption;

namespace EI.RP.Stubs.CoreServices.Encryption
{
	public class NoEncryptionService : IEncryptionService
	{
		public async Task<string> EncryptAsync(string source, bool addPrefix = false)
		{
			return source;
		}

		public async Task<string> DecryptAsync(string source, bool onlyIfHasPrefix = false)
		{
			return source;
		}

		public Task<TModel> DecryptModelAsync<TModel>(TModel request) where TModel : class
		{
			return Task.FromResult(request);
		}

		public string GetSha1(string source)
		{
			return source;
		}
	}
}

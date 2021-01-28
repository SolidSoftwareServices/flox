using System;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Secrets
{
	public interface ISecretsRepository
	{


		Task<bool> ContainsAsync(string key);

		Task<string> GetAsync(string key, int maxAttempts,bool cacheResults=true, CancellationToken cancellationToken=default(CancellationToken));

		Task SetAsync(string key,string value);

		Task RemoveAsync(string key);
	}
}

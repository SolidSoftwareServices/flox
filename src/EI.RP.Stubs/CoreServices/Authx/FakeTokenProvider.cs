using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;

namespace EI.RP.Stubs.CoreServices.Authx
{
	public class FakeTokenProvider:IBearerTokenProvider
	{
		public Task<string> GetToken(string resource, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(Guid.NewGuid().ToString());
		}

		public Task AppendHeaders(HttpRequestHeaders headers, string bearerTokenResource,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}
	}
}

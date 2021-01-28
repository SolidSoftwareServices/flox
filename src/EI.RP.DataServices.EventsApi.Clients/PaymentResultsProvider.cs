using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.EventsApi.Clients.Config;

namespace EI.RP.DataServices.EventsApi.Clients
{
	internal sealed class PaymentResultsProvider : IPaymentResultsProvider,IDisposable
	{
		private readonly JsonApiClient _client;

		public PaymentResultsProvider(IPaymentResultsProviderSettings settings, IProfiler profiler,IHttpClientBuilder httpClientBuilder)
		{
			_client = new JsonApiClient(httpClientBuilder,settings.PaymentResultsProviderUrl, profiler);
		}

		public async Task<bool> WasPaymentCompletedSuccessfullyToday(string accountNumber)
		{
			var response = await _client.GetAsync($"/events/makepayment?inputDate={DateTime.Now:dd/MM/yyyy}&conAcc={accountNumber}");

			return bool.Parse(response);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				_client?.Dispose();
			}
		}

		~PaymentResultsProvider()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
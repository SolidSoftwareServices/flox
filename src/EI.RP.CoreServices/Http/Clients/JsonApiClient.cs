using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System.Async;

namespace EI.RP.CoreServices.Http.Clients
{
    public class JsonApiClient:IDisposable
    {
        private readonly AsyncLazy<HttpClient> _client;

        public JsonApiClient(IHttpClientBuilder httpClientBuilder, string baseUrl, IProfiler profiler, IBearerTokenProvider bearerTokenProvider=null,string bearerTokenResource=null)
        {
	        if (bearerTokenResource != null && bearerTokenProvider == null)
	        {
		        throw new ArgumentNullException(nameof(bearerTokenProvider));
	        }
	        _client = new AsyncLazy<HttpClient>(async() =>
	        {
		       return  await httpClientBuilder.BuildAsync(null, async c =>
		        {
			        c.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
			        if (bearerTokenResource != null)
			        {
				        await bearerTokenProvider.AppendHeaders(c.DefaultRequestHeaders, bearerTokenResource);
			        }
		        });
	        });
		}


        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value.Dispose();
            }
        }

        private async Task SetDefaultHeaders()
        {
	        var clientValue = await _client.Value;
	        clientValue.DefaultRequestHeaders.Accept.Clear();
            
            clientValue.DefaultRequestHeaders.AddIfNotExistsSeparatedByComma("x-csrf-token", "fetch");
        }

        public async Task PostJsonAsync<TMessage>(string relativeUrl, TMessage eventToPublish)
            where TMessage : IEventApiMessage
        {
            await SetDefaultHeaders();
            var clientValue = await _client.Value;
            clientValue.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			HttpResponseMessage response = null;

            try
            {
	            var jsonMessage = eventToPublish.ToJson();
	            response = await clientValue.PostAsync(relativeUrl, new StringContent(jsonMessage,Encoding.UTF8,"application/json"));
                response.EnsureSuccessStatusCode();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new Exception($"response :{responseContent}", ex);
                }

                throw;
            }
        }


        public async Task<string> GetAsync(string relativeUrl)
        {
			await SetDefaultHeaders();
			var clientValue = await _client.Value;
			var response = await clientValue.GetAsync(relativeUrl);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
        }
    }
}
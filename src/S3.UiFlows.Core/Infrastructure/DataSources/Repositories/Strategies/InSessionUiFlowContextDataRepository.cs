using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.Http.Session;
using S3.CoreServices.Serialization;
using Newtonsoft.Json;

namespace S3.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies
{
	internal class InSessionUiFlowContextDataRepository : IInternalUiFlowContextRepository
	{
		private const string KeyPrefix = "_._uiflow__.";
		private readonly IUserSessionProvider _userSessionProvider;

		public InSessionUiFlowContextDataRepository(IUserSessionProvider userSessionProvider)
		{
			_userSessionProvider = userSessionProvider;
		}

		public ContextStoreStrategy StoreStrategy { get; } = ContextStoreStrategy.InSession;

		public async Task<UiFlowContextData> LoadByFlowHandler(string flowHandler)
		{
			ThrowIfNotSupported();

			var data = await _userSessionProvider.GetAsync<string>(ResolveKey(flowHandler));
			try
			{
				var uiFlowContextData = data.JsonToObject<UiFlowContextData>(true);
				return uiFlowContextData;
			}
			catch (JsonSerializationException exception)
			{
				throw new Exception(data, exception);
			}
		}


		public async Task<UiFlowContextData> Save(UiFlowContextData contextData)
		{
			ThrowIfNotSupported();

			var jsonData = contextData.ToJson(true);
			await _userSessionProvider.SetAsync(ResolveKey(contextData.FlowHandler), jsonData);

			return contextData;
		}

		public async Task Remove(string flowHandler)
		{
			ThrowIfNotSupported();
			await _userSessionProvider.RemoveAsync(ResolveKey(flowHandler));
		}

		public async Task<IEnumerable<UiFlowContextData>> GetAll()
		{
			ThrowIfNotSupported();
			var keys = (await _userSessionProvider.GetAllKeysAsync()).Where(x => x.StartsWith(KeyPrefix));
			return await Task.WhenAll(keys
				.Select(x => LoadByFlowHandler(x.Remove(0, KeyPrefix.Length))));
		}

		private void ThrowIfNotSupported()
		{
			if (_userSessionProvider.IsAnonymous())
				throw new InvalidOperationException("The session repository is not supported for anonymous requests"){Source = "Session"};
		}

		private string ResolveKey(string flowHandler)
		{
			return $"{KeyPrefix}{flowHandler}";
		}
	}
}
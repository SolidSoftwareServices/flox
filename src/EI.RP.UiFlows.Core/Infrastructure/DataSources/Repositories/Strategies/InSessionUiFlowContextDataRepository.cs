using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Serialization;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies
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

			var data = _userSessionProvider.Get<string>(ResolveKey(flowHandler));
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


		public Task<UiFlowContextData> Save(UiFlowContextData contextData)
		{
			ThrowIfNotSupported();

			var jsonData = contextData.ToJson(true);
			_userSessionProvider.Set(ResolveKey(contextData.FlowHandler), jsonData);

			return Task.FromResult(contextData);
		}

		public async Task Remove(string flowHandler)
		{
			ThrowIfNotSupported();
			_userSessionProvider.Remove(ResolveKey(flowHandler));
		}

		public async Task<IEnumerable<UiFlowContextData>> GetAll()
		{
			ThrowIfNotSupported();
			return await Task.WhenAll(_userSessionProvider.GetAllKeys().Where(x => x.StartsWith(KeyPrefix))
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
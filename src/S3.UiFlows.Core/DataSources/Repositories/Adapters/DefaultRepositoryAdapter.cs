using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using S3.CoreServices.Serialization;

namespace S3.UiFlows.Core.DataSources.Repositories.Adapters
{
	internal class DefaultRepositoryAdapter : IRepositoryAdapter
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IFlowsStore _store;
		

		public DefaultRepositoryAdapter(IFlowsStore store)
		{
			_store = store;
		}
		


		public async Task<UiFlowContextData> LoadByFlowHandler(string flowHandler)
		{
			try
			{
				var contextData = (await _store.GetAsync(flowHandler)).JsonToObject<UiFlowContextData>(true);
				return contextData;
			}
			catch (JsonSerializationException exception)
			{
				throw new Exception(flowHandler, exception);
			}
			
		}

		public async Task<UiFlowContextData> Save(UiFlowContextData contextData)
		{
			var jsonData = contextData.ToJson(true);
			await _store.SetAsync(contextData.FlowHandler, jsonData);
			Logger.Debug(()=>$"Added {contextData.FlowHandler}");
			return contextData;
		}

		public async Task Remove(string flowHandler)
		{
			await _store.RemoveAsync(flowHandler);
			Logger.Debug(() => $"Removed {flowHandler}");
		}

		public async Task<IEnumerable<UiFlowContextData>> GetAll()
		{
			return (await _store.GetValuesAsync()).Select(x => x.JsonToObject<UiFlowContextData>(true)).ToArray();
		}

		internal async Task Reset()
		{
			await _store.ClearAsync();
			Logger.Debug(()=>"Cleared all");
		}
	}
}
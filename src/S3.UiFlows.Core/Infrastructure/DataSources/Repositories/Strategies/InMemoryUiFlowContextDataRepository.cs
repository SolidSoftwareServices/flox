using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.Serialization;
using Newtonsoft.Json;
using NLog;

namespace S3.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies
{
	internal class InMemoryUiFlowContextDataRepository : IInternalUiFlowContextRepository
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly ConcurrentDictionary<string, string> Store = new ConcurrentDictionary<string, string>();

		internal UiFlowContextData[] Values =>
			Store.Values.Select(x => x.JsonToObject<UiFlowContextData>(true)).ToArray();

		public ContextStoreStrategy StoreStrategy { get; } = ContextStoreStrategy.InMemoryOfSingleInstance;


		public async Task<UiFlowContextData> LoadByFlowHandler(string flowHandler)
		{
			try
			{
				var contextData = Store[flowHandler].JsonToObject<UiFlowContextData>(true);
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
			Store.AddOrUpdate(contextData.FlowHandler, a => jsonData, (a, b) => jsonData);
			Logger.Debug(()=>$"Added {contextData.FlowHandler}");
			return contextData;
		}

		public async Task Remove(string flowHandler)
		{
			if (Store.TryRemove(flowHandler, out var json))
			{
				Logger.Debug(()=>$"Removed {flowHandler}");
			}
		}

		public async Task<IEnumerable<UiFlowContextData>> GetAll()
		{
			return Values;
		}

		internal void Reset()
		{
			Store.Clear();
			Logger.Debug(()=>"Cleared all");
		}
	}
}
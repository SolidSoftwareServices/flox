using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Registry;

namespace S3.UiFlows.Mvc.Infrastructure
{
	internal class FlowsRegistry : IFlowsRegistry
	{
		public static readonly FlowsRegistry Instance = new FlowsRegistry();
		private  Lazy<Dictionary<string,FlowRegister>> _flows;

		private FlowsRegistry()
		{
		}

		public IEnumerable<FlowRegister> AllFlows
		{
			get
			{
				var allFlows = _flows ??
				               throw new InvalidOperationException("Discover the flows by using load before this operation.");
				return allFlows.Value.Values;
			}
		}

		public FlowsRegistry Load(Assembly flowsAssembly, string flowsRootNamespace,string flowsRootPath)
		{
			flowsRootPath = flowsRootPath.EndsWith('/') ? flowsRootPath : $"{flowsRootPath}/";
			_flows = new Lazy<Dictionary<string,FlowRegister>>(() => TypesFinder.Resolver
				.FindTypesByNamespace(flowsAssembly, flowsRootNamespace)
				.Where(x => x.Implements<IUiFlowScreen>() || x.Implements<IUiFlowInitializationStep>())
				.Select(x =>
					x.Namespace.Replace($"{flowsRootNamespace}.", string.Empty).Split('.').First()
				)
				.Distinct()
				.Select(x => new FlowRegister(x,flowsRootPath,flowsRootNamespace)).ToDictionary(x=>x.Name.ToLowerInvariant(),x=>x));
				
			return this;
		}

		public FlowRegister GetByName(string name, bool failIfNotFound = false)
		{
			var key = name.ToLowerInvariant();
			if (!_flows.Value.ContainsKey(key))
			{
				if (failIfNotFound) throw new ArgumentOutOfRangeException(nameof(name));
				return null;
			}

			return _flows.Value[key];
		}

		private static readonly ConcurrentDictionary<Type, FlowRegister> GetByTypeCache =
			new ConcurrentDictionary<Type, FlowRegister>();
		public FlowRegister GetByType(Type type, bool failIfNotFound = false)
		{
			return GetByTypeCache.GetOrAdd(type,()=> AllFlows.SingleOrDefault(x => x.IsFlowType(type)));
		}
	}
}
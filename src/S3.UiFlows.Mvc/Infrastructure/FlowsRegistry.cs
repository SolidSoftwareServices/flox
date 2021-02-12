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
		private readonly Dictionary<string,FlowRegister> _flows;

		public FlowsRegistry(Assembly flowsAssembly, string flowsRootNamespace, string flowsRootPath)
		{
			flowsRootPath = flowsRootPath.EndsWith('/') ? flowsRootPath : $"{flowsRootPath}/";
			_flows = TypesFinder.Resolver
				.FindTypesByNamespace(flowsAssembly, flowsRootNamespace)
				.Where(x => x.Implements<IUiFlowScreen>() || x.Implements<IUiFlowInitializationStep>())
				.Select(x =>
					x.Namespace.Replace($"{flowsRootNamespace}.", string.Empty).Split('.').First()
				)
				.Distinct()
				.Select(x => new FlowRegister(x, flowsRootPath, flowsRootNamespace))
				.ToDictionary(x => x.Name.ToLowerInvariant(), x => x);

		}

		public IEnumerable<FlowRegister> AllFlows
		{
			get
			{
				var allFlows = _flows ??
				               throw new InvalidOperationException("Discover the flows by using load before this operation.");
				return allFlows.Values;
			}
		}

	
		public FlowRegister GetByName(string name, bool failIfNotFound = false)
		{
			var key = name.ToLowerInvariant();
			if (!_flows.ContainsKey(key))
			{
				if (failIfNotFound) throw new ArgumentOutOfRangeException(nameof(name));
				return null;
			}

			return _flows[key];
		}

		private static readonly ConcurrentDictionary<Type, FlowRegister> GetByTypeCache =
			new ConcurrentDictionary<Type, FlowRegister>();
		public FlowRegister GetByType(Type type, bool failIfNotFound = false)
		{
			var flowRegister = GetByTypeCache.GetOrAdd(type,(t)=> AllFlows.SingleOrDefault(x => x.IsFlowType(t)));
			return flowRegister;
		}
	}
}
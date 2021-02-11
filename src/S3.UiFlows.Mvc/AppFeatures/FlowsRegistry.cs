using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Mvc.AppFeatures
{
	internal class FlowsRegistry
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
				.Select(x => new FlowRegister(x,flowsRootPath)).ToDictionary(x=>x.Name.ToLowerInvariant(),x=>x));
				
			return this;
		}

		public FlowRegister GetByName(string name)
		{
			var key = name.ToLowerInvariant();
			if (!_flows.Value.ContainsKey(key))
			{
				return null;
			}

			return _flows.Value[key];
		}
	}
}
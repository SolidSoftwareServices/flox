using System;
using System.Collections.Concurrent;

namespace S3.UiFlows.Core.Registry
{
	public class FlowRegister
	{
		private readonly string _flowsRootNamespace;

		internal FlowRegister(string name, string flowsRootPath, string flowsRootNamespace)
		{
			if (flowsRootPath == null) throw new ArgumentNullException(nameof(flowsRootPath));
			_flowsRootNamespace = flowsRootNamespace ?? throw new ArgumentNullException(nameof(flowsRootNamespace));
			Name = name ?? throw new ArgumentNullException(nameof(name));
			UrlPath = $"{flowsRootPath}{name}";
		}

		public string UrlPath { get; }

		public string Name { get; }

		private string _componentsUrlPath;
		public string ComponentsUrlPath => _componentsUrlPath ??= $"{UrlPath}/Components";

		private readonly ConcurrentDictionary<Type, bool> _isFlowTypeCache = new ConcurrentDictionary<Type, bool>();
		public bool IsFlowType(Type type)
		{
			return _isFlowTypeCache.GetOrAdd(type, (t) =>
			{
				if (t.Namespace == null) return false;
				var removedPrefix = t.Namespace.Replace($"{_flowsRootNamespace}.{Name}", string.Empty);
				return removedPrefix.StartsWith(".") || removedPrefix == string.Empty;
			});
		}

		public override string ToString()
		{
			return $"{nameof(Name)}: {Name}";
		}
	}
}
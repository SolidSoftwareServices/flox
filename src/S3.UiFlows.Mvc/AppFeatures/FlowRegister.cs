using System;

namespace S3.UiFlows.Mvc.AppFeatures
{
	internal class FlowRegister
	{

		public FlowRegister(string name, string flowsRootPath)
		{
			if (flowsRootPath == null) throw new ArgumentNullException(nameof(flowsRootPath));
			Name = name ?? throw new ArgumentNullException(nameof(name));
			UrlPath = $"{flowsRootPath}{name}";
		}

		public string UrlPath { get;  }

		public string Name { get; }
		
		private string _componentsUrlPath;
		public string ComponentsUrlPath => _componentsUrlPath ??= $"{UrlPath}/Components";
	}
}
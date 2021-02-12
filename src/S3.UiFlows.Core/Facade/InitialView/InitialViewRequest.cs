using System;
using System.Dynamic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Facade.InitialView
{
	public class InitialViewRequest<TResult>
	{
		public string FlowType { get; set; }
		public string ContainerFlowHandler { get; set; }
		public string StartType { get; set; }

		//TODO: refactor
		public ExpandoObject FlowInput { get; set; }

		public Func<UiFlowScreenModel, Task<TResult>> OnBuildView { get; set; }


	}
}

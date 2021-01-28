using System;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.UiFlows.Core.Facade.InitialView
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

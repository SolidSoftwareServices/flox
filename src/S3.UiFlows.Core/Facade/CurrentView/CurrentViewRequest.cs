using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Facade.Delegates;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.UiFlows.Core.Facade.CurrentView
{
	public class CurrentViewRequest<TResult>
	{
		public string FlowHandler { get; set; }

		public IDictionary<string, object> ViewParameters { get; set; }

		public Func<Task<TResult>> BuildResultOnFlowNotFound { get; set; }

		/// <summary>
		/// flow handler to redirect the request to
		/// </summary>
		public BuildResultOnRequestRedirectToDelegate<TResult> BuildResultOnRequestRedirectTo { get; set; }

		public AddModelErrorDelegate OnAddModelError { get; set; }

		public Func<BuildViewInput,Task<TResult>> OnBuildView { get; set; }
		public bool ResolveAsComponentOnly { get; set; }

		public class BuildViewInput
		{
			public string ViewName { get; set; }
			public UiFlowScreenModel ScreenModel { get; set; }
			public bool BuildPartial { get; set; }
			public string FlowType { get; set; }
		}

		public Func<CallbackOriginalFlow, Task<TResult>> OnCallbackCallerFlow {get;set; }
	}
}

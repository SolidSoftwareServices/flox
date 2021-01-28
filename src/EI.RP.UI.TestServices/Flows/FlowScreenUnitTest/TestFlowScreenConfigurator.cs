using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UI.TestServices.Flows.Shared;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.UI.TestServices.Flows.FlowScreenUnitTest
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	internal class TestFlowScreenConfigurator : TestFlowNavigationHelper, IInternalScreenFlowConfigurator
	{
		private readonly IUiFlowScreen _step;

		public TestFlowScreenConfigurator(IUiFlowScreen step) : base(step.ScreenStep)
		{
			_step = step;
		}

		public void OnEntry(Func<Task> action, string entryActionDescription = null)
		{
			throw new NotSupportedException();
		}

		public void AddErrorTransitionIfUndefined()
		{
			if (!NavigationResolvers.ContainsKey(ScreenEvent.ErrorOccurred))
				//Add default reentry on error
				OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

		public string ScreenName => _step.ToString();

		public IEnumerable<ScreenTransition> Transitions =>
			NavigationResolvers.Select(x => new ScreenTransition(x.Key, x.Value.Resolve()));

		protected override ScreenName OnExecute(NavigationResolver navigationResolver)
		{
			return navigationResolver.Resolve();
		}
	}
}
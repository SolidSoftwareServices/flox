using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Configuration
{
	internal interface IInternalScreenFlowConfigurator : IScreenFlowConfigurator
	{
		string ScreenName { get; }

		IEnumerable<ScreenTransition> Transitions { get; }
		void OnEntry(Func<Task> action, string entryActionDescription = null);

		void AddErrorTransitionIfUndefined();
	}
}
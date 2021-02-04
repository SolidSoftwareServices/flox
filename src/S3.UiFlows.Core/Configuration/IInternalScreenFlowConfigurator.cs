using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Configuration
{
	internal interface IInternalScreenFlowConfigurator : IScreenFlowConfigurator
	{
		string ScreenName { get; }

		IEnumerable<ScreenTransition> Transitions { get; }
		IReadOnlyDictionary<ScreenEvent, Func<ScreenEvent, IUiFlowContextData, Task>> Handlers { get; }
		void OnEntry(Func<Task> action, string entryActionDescription = null);

		void AddErrorTransitionIfUndefined();
	}
}
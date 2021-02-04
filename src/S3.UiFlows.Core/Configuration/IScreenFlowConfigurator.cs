using System;
using System.Threading.Tasks;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Configuration
{
	/// <summary>
	///     it configures the uiflow for a single screen
	/// </summary>
	public interface IScreenFlowConfigurator
	{
		IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo);
		IScreenFlowConfigurator OnEventsNavigatesTo(ScreenEvent[] whenEvents, ScreenName navigatesTo);

		IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo,
			Func<bool> whenConditionMatches, string conditionFriendlyDescription);
		IScreenFlowConfigurator OnEventNavigatesToAsync(ScreenEvent whenEvent, ScreenName navigatesTo,
			Func<Task<bool>> whenConditionMatches, string conditionFriendlyDescription);
		IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent);
		IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents);

		IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent, Func<bool> whenConditionMatches,
			string conditionFriendlyDescription);

		IScreenFlowConfigurator OnEventReentriesCurrentAsync(ScreenEvent whenEvent, Func<Task<bool>> whenConditionMatches,
			string conditionFriendlyDescription);

		IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents, Func<bool> whenConditionMatches,
			string conditionFriendlyDescription);

		IScreenFlowConfigurator SubStepOf(ScreenName step);
		IScreenFlowConfigurator OnEventExecutes(ScreenEvent screenEvent, Func<ScreenEvent, IUiFlowContextData, Task> eventHandler);
		IScreenFlowConfigurator OnEventExecutes(ScreenEvent screenEvent, Action<ScreenEvent, IUiFlowContextData> eventHandler);
	}
}
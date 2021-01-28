using System;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Configuration
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
	}
}
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Infrastructure.StateMachine
{
	public interface IStateMachine
	{
		ScreenName CurrentStep { get; }
	}
}
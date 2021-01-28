using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Infrastructure.StateMachine
{
	public interface IStateMachine
	{
		ScreenName CurrentStep { get; }
	}
}
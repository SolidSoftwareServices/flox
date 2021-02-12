using System.Threading.Tasks;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Infrastructure.StateMachine
{
	internal interface IInternalStateMachine : IStateMachine
	{
		bool WasCreated { get; }
		IInternalStateMachine BuildNew(ScreenName initialStep);
		Task ExecuteTrigger(ScreenEvent transitionTrigger);

		IInternalScreenFlowConfigurator Configure(ScreenName step);
		string ToDotGraph();
		void Reset();
	}
}
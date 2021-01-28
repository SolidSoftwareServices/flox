using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Infrastructure.StateMachine
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
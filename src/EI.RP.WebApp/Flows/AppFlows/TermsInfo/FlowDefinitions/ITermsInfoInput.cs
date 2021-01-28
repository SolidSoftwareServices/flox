using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.TermsInfo.FlowDefinitions
{
	public interface ITermsInfoInput : IFlowInputContract
	{
		TermsInfoFlowInitializer.StartType StartType { get; set; }
	}

    public class TermsInfoInput : ITermsInfoInput
    {
        public TermsInfoFlowInitializer.StartType StartType { get; set; }
	}
}
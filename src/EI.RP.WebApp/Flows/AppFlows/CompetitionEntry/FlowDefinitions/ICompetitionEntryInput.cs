using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions
{
    public interface ICompetitionEntryInput : IFlowInputContract
    {
        string AccountNumber { get; set; }
    }

    public class CompetitionEntryInput : ICompetitionEntryInput
    {
        public string AccountNumber { get; set; }
    }
}

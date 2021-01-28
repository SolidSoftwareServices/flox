using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions
{
	public interface IAccountsFlowInput : IFlowInputContract
	{
        string AccountTypeValue { get; set; }
        bool IsOpen { get; set; }
        int PageIndex { get; set; }
        string Source { get; set; }
    }

	public class AccountsFlowInput : IAccountsFlowInput
    {
        public string AccountTypeValue { get; set; }
        public bool IsOpen { get; set; }
        public int PageIndex { get; set; }
        public string Source { get; set; }
    }
}
using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions
{
	public interface IContactUsInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
	}

    public class ContactUsInput : IContactUsInput
    {
		public string AccountNumber { get; set; }
	}
}
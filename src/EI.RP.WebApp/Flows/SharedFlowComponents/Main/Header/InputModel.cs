namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Header
{
	public class InputModel
	{
		public string AccountNumber { get; set; }
		public string Title { get; set; } = null;
		public bool ShowAccountNumber { get; set; } = true;
		public bool ShowAddress { get; set; } = true;
	}
}

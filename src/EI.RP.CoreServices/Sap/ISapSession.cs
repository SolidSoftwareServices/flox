namespace EI.RP.CoreServices.Sap
{
	public interface ISapSession
	{
		string SapCsrf { get; set; }
		string SapJsonCookie { get; set; }
	}
}
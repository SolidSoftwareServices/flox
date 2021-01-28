namespace EI.RP.DataServices.SAP.Clients.Infrastructure.Session
{
	public interface ISapSessionDataRepository
    {
        string SapCsrf { get; set; }
        string SapJsonCookie { get; set; }
    }
}

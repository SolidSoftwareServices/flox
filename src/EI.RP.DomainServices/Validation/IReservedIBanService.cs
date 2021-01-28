namespace EI.RP.DomainServices.Validation
{
	public interface IReservedIbanService
	{
		bool IsReservedIban(string iban);
	}
}
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Platform.SendEmail
{
	public interface IEmailSettings
	{
		Task<string> ResidentialOnlineEmailRecipientAsync();
		Task<string> AccountQueryCcEmailAsync();
	}
}
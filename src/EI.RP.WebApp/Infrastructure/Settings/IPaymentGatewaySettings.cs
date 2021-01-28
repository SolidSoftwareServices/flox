using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Settings
{
	public interface IPaymentGatewaySettings
	{
		
		bool PaymentGatewayAutoSettle { get; }
		string PaymentGatewayRequestUrl { get; }
		Task<string> PaymentGatewaySecretAsync();
		Task<string> PaymentGatewayAccountAsync();
		Task<string> PaymentGatewayMerchantIdAsync();
	}
}
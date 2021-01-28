using System.Threading.Tasks;

namespace EI.RP.DataServices
{
	public interface IPaymentResultsProvider : IDataService
	{
		Task<bool> WasPaymentCompletedSuccessfullyToday(string accountNumber);
	}
}
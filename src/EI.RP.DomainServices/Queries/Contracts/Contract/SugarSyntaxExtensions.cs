using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;

namespace EI.RP.DomainServices.Queries.Contracts.Contract
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<ContractItem> GetContactByAccountNumber(this IDomainQueryResolver provider,
			string accountNumber, bool byPassPipeline = false)
		{
			if (accountNumber == null) return null;

			var contractItems = await provider
				.FetchAsync<ContractInfoQuery, ContractItem>(new ContractInfoQuery
				{
					AccountNumber = accountNumber
				}, byPassPipeline);
			return contractItems.SingleOrDefault();
		}		
	}
}
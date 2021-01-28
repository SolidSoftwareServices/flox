using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Contracts.CanCloseAccount
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<AccountsCloseableInfo> CanCloseAccounts(this IDomainQueryResolver provider,
			string electricityAccountNumber, string gasAccountNumber, DateTime closingDate, bool byPassPipeline = false)
		{

			var result = await provider
				.FetchAsync<CanCloseAccountQuery, AccountsCloseableInfo>(new CanCloseAccountQuery
				{
					ElectricityAccountNumber = electricityAccountNumber,GasAccountNumber=gasAccountNumber,ClosingDate = closingDate
				}, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}
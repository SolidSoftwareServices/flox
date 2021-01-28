using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<CheckMoveOutRequestResult> CheckMoveOut(
			this IDomainQueryResolver provider,
			string contractId,
			bool byPassPipeline = false)
		{
			if (contractId == null) throw new ArgumentNullException(nameof(contractId));

			var query = new CheckMoveOutRequestResultQuery
			{
				ContractID = contractId
			};

			return (await provider.FetchAsync<CheckMoveOutRequestResultQuery, CheckMoveOutRequestResult>(query, byPassPipeline)).Single();
		}
	}
}
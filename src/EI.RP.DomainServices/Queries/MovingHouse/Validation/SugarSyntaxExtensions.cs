using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MoveHouse;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<MovingHouseRulesValidationResult>> GetMovingHouseValidationResult(
			this IDomainQueryResolver provider,
			string electricityAccountNumber,
			string gasAccountNumber, 
			string newMPRN)
		{
			var query = new MovingHouseValidationQuery
			{
				ElectricityAccountNumber = electricityAccountNumber,
				GasAccountNumber = gasAccountNumber,
				NewMPRN = newMPRN
			};

			return await provider.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
		}
	}
}
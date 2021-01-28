using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Queries.MovingHouse.InstalmentPlans
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<InstalmentPlanRequestResult> CheckInstalmentPlan(
			this IDomainQueryResolver provider,
			string accountNumber,
			bool byPassPipeline = false)
		{
			if (accountNumber == null) throw new ArgumentNullException(nameof(accountNumber));

			var query = new InstalmentPlanQuery
            {
				AccountNumber = accountNumber
            };

			return (await provider.FetchAsync<InstalmentPlanQuery, InstalmentPlanRequestResult>(query, byPassPipeline)).Single();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.SmartActivation;

namespace EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static Task<IEnumerable<SmartPlan>>
			GetSmartActivationPlans(this IDomainQueryResolver provider,string accountNumber, bool onlyActive=true, bool byPassPipeline = false)
		{
			return provider.FetchAsync<SmartActivationPlanQuery,SmartPlan> (new SmartActivationPlanQuery
			{
				AccountNumber = accountNumber,
				OnlyActive=onlyActive
			},byPassPipeline);
		}

		public static async Task<SmartPlan>
			GetSmartActivationPlan(this IDomainQueryResolver provider,string accountNumber, string planName, bool onlyActive=true, bool byPassPipeline = false)
		{
			var plans = await provider.GetSmartActivationPlans(accountNumber, onlyActive, byPassPipeline);
			return plans.SingleOrDefault(x => x.PlanName.Equals(planName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}


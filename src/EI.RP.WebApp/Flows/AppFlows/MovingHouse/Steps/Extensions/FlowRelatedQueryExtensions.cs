using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions
{
	public static class  FlowRelatedQueryExtensions
	{
		public static async Task<AccountInfo> GetSecondaryAccount(this IDomainQueryResolver provider,
			FlowInitializer.RootScreenModel model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			AccountInfo result = null;
			if (model.HasSecondaryAccount())
			{
				result=await provider.GetAccountInfoByAccountNumber(model.SecondaryAccountNumber());
			}

			return result;
		}

		public static async Task<AccountInfo[]> GetFlowAccounts(this IDomainQueryResolver provider,
			FlowInitializer.RootScreenModel model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			var result = await provider.GetDuelFuelAccountsByAccountNumber(model.InitiatedFromAccountNumber);
			return result as AccountInfo[] ?? result.ToArray();
		}
		public static async Task<AccountInfo> GetFlowElectricityAccount(this IDomainQueryResolver provider,
			FlowInitializer.RootScreenModel model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			var result = await provider.GetFlowAccounts(model);
			return result.SingleOrDefault(x=>x.IsElectricityAccount());
		}
	}
}

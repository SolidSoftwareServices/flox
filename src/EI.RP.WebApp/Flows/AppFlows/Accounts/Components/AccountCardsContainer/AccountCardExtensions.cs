using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer
{
	public static class AccountCardExtensions
	{
		public static  bool ResolveIsMultipageView(this IEnumerable<AccountOverview> accounts,bool isOpen)
		{
			if (accounts == null) throw new ArgumentNullException(nameof(accounts));

			var accountsArray = accounts.Where(x => x.IsOpen == isOpen || x.IsPendingOpening()).ToArray();

			var elecCount = accountsArray.Count(x => x.ClientAccountType == ClientAccountType.Electricity);
			var gasCount = accountsArray.Count(x => x.ClientAccountType == ClientAccountType.Gas);
			var esCount = accountsArray.Count(x => x.ClientAccountType == ClientAccountType.EnergyService);

			var hasManyElectricityAccounts = elecCount > 1;
			var hasManyGasAccounts = gasCount > 1;
			var hasManyEnergyServiceAccounts = esCount > 1;

			return hasManyElectricityAccounts && accountsArray.Length > elecCount ||
			       hasManyGasAccounts && accountsArray.Length > gasCount ||
			       hasManyEnergyServiceAccounts && accountsArray.Length> esCount;
		}
	}
}
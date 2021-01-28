using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Plan.Components.PlanDetails;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Components.PlanDetails
{
	[TestFixture]
	internal class PlanDetails_ViewModelBuilderTest : UnitTestFixture<PlanPageComponentTestContext<ViewModelBuilder>,
			ViewModelBuilder>
	{
		[TestCase("a b c",ExpectedResult = "a b c")]
		[TestCase("",ExpectedResult = "")]
		public async Task<string> ItMapsThePlanName(string whenNameIs)
		{
			var actual = await Context
				.WithPlanName(whenNameIs)
				.Sut.Resolve(new InputModel {AccountNumber = Context.AccountNumber });

			return actual.PlanName;
		}

		[TestCase(0, ExpectedResult = 0)]
		
		[TestCase(10, ExpectedResult = 10)]
		[TestCase(-10, ExpectedResult = -10)]
		[TestCase(1.555, ExpectedResult =1.555)]
		[TestCase(-5.55, ExpectedResult = -5.55)]
		public async Task<decimal> ItMapsThePlanDiscount(decimal discount)
		{
			var actual = await Context
				.WithDiscount(discount)
				.Sut.Resolve(new InputModel { AccountNumber = Context.AccountNumber });

			return actual.Discount;
		}

		public static IEnumerable<TestCaseData> ItResolvesTheUpgradeInfoCases()
		{
			var bools = new[] {true, false};
			foreach (var isOpen in bools)
			foreach (var hasElectricity in bools)
			foreach (var hasGas in bools)
			{
				if (!hasElectricity && !hasGas) continue;
				foreach (var isSmartActivationEnabled in bools)
				foreach (var isAlreadySmart in bools)
				foreach (var canBeSmart in bools)
				{
					if (isAlreadySmart && canBeSmart) continue;
					foreach (var selectedAccount in new[] {ClientAccountType.Electricity, ClientAccountType.Gas})
					{
						if (selectedAccount.IsElectricity() && !hasElectricity
						    || selectedAccount.IsGas() && !hasGas) continue;
						yield return new TestCaseData(hasElectricity, hasGas, isSmartActivationEnabled, isAlreadySmart, canBeSmart,
								selectedAccount,isOpen)
							.SetName(ResolveName())
							.Returns(ResolveExpectedResult());

						ViewModel.UpgradeInfo ResolveExpectedResult()
						{
							var info = new ViewModel.UpgradeInfo();
							info.IsUpgradeToSmartAvailable = isSmartActivationEnabled && isOpen &&  !isAlreadySmart && canBeSmart && hasElectricity && selectedAccount.IsElectricity();
							if (info.IsUpgradeToSmartAvailable)
							{
								if (isOpen && selectedAccount.IsElectricity() && hasElectricity && hasGas)
									info.UpgradePlanName = "Home Dual+";
								else if (selectedAccount.IsElectricity() && hasElectricity && !hasGas)
									info.UpgradePlanName = "Home Electric+";
								else
									info.UpgradePlanName = null;
							}

							return info;
						}

						string ResolveName()
						{
							var separator = hasGas && hasElectricity ? "," : string.Empty;
							var e = hasElectricity ? "Electricity" : string.Empty;
							var g = hasGas ? "Gas" : string.Empty;
							var s = isAlreadySmart ? string.Empty : "NOT";
							var c = canBeSmart ? ",BUT can be made smart" : string.Empty;
							var sae = isSmartActivationEnabled ? string.Empty : "NOT";
							var o = isOpen ? "" : "NOT";
									return $"Accounts:[{e}{separator}{g}] SmartActivation {sae} enabled, selected {selectedAccount}  is {s} smart {c} and electricity is {o} opened";
						}
					}
				}
			}
		}

		[TestCaseSource(nameof(ItResolvesTheUpgradeInfoCases))]
		public async Task<ViewModel.UpgradeInfo> ItResolvesTheUpgradeInfo(bool hasElectricity,bool hasGas,bool isSmartActivationEnabled, bool isAlreadySmart,bool canBeSmart,ClientAccountType selectedAccount,bool isOpen)
		{
			var actual=await Context
				.WithAccounts(hasElectricity, hasGas,isElectricityOpen:isOpen)
				.WithSelectedAccount(selectedAccount)
				.WithSmartConfiguration(isAlreadySmart, canBeSmart)
				.WithIsSmartActivationEnabled(isSmartActivationEnabled)
				.Sut.Resolve(new InputModel {AccountNumber = Context.AccountNumber});

			return actual.Upgrading;

		}

		
	}
}
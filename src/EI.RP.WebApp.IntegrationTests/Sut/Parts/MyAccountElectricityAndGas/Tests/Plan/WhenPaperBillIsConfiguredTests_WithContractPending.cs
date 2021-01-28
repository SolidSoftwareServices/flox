using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenPaperBillIsConfiguredTests_WithContractPending:WhenInPlanPageTests
	{
		protected override bool IsSmartAccount => false;
		protected override bool IsUpgradeToSmartAvailable => true;
		protected override bool WithPaperBill => true;
		protected override bool WithIsContractPending => true;
		

		[Test]
		public async Task TheViewShowsTheExpectedInformation()
		{
			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			Assert.IsNull(Sut.PaperlessBillingLink);
			Assert.IsNotNull(Sut.PaperlessBillingDisabledText);
			Assert.IsTrue(Sut.PaperlessBillSelected);
			Assert.IsFalse(Sut.PaperBillSelected);
			Assert.IsTrue(Sut.PaperlessBillingToggle.HasAttribute("disabled"));
		}
	}
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_CanSmartMoveToStandardPlan_WithContractPending : WhenInPlanPageTests
	{
		protected override bool CanSmartMeterMoveToToStandardPlan => true;
		protected override bool WithIsContractPending => true;

		[Test]
		public async Task CanSeeComponents()
		{
			var account = UserConfig.Accounts.First();

			Assert.IsNotNull(Sut.SmartMeterDataHeading);
			Assert.IsNotNull(Sut.SmartMeterDataToggle);
			Assert.IsTrue(Sut.SmartMeterDataToggle.HasAttribute("disabled"));
			Assert.IsNotNull(Sut.SmartMeterDataText);
			Assert.IsNotNull(Sut.SmartMeterDataDowngradCheckBoxText);

			var downgradeToPlan = string.IsNullOrEmpty(account.BundleReference) ? "Home Electric" : "Home Dual";
			var downgradeToPlanText = $"I will be moved to the standard {downgradeToPlan} plan with the same pricing and discounts as my current plan";
			Assert.IsTrue(Sut.SmartMeterDataDowngradCheckBoxText.InnerHtml.Contains(downgradeToPlanText));

			Assert.IsTrue(Sut.SmartMeterDataTermsAndConditionsLink?.Href?.Equals("https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing"));
		}
	}

}

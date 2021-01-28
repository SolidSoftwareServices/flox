using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_UpgradeToSmartAvailableTest : WhenInPlanPageTests
	{
		protected override bool IsSmartAccount => false;
		protected override bool IsUpgradeToSmartAvailable => true;

		[Test]
		public void UpgradeToSmartAvailable()
		{
			var updagradePlanName = UserConfig.Accounts.Count() == 1 ? "Home Electric+" : "Home Dual+";

			Assert.IsTrue(Sut.UpgradeToSmartHeading?.TextContent.Equals("Upgrade to a smart plan for free"));
			Assert.IsTrue(Sut.UpgradeToSmartText?.TextContent.Equals($"With {updagradePlanName}, you get advanced electricity insights, monthly billing and much more at no extra cost."));
			Assert.IsTrue(Sut.UpgradeToSmartLink?.TextContent.Equals($"Find out more about {updagradePlanName}"));
		}

		[Test]
		public async Task NavigateToSmartActivation()
		{
			Assert.IsNotNull(Sut.UpgradeToSmartLink);
			var page = (await Sut.ClickOnElement(Sut.UpgradeToSmartLink)).CurrentPageAs<Step1EnableSmartFeaturesPage>();
			(await page.ClickOnElement(page.CancelButton)).CurrentPageAs<PlanPage>();
		}
	}
}
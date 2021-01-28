using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_CanAddGasTest : WhenInPlanPageTests
	{
		protected override bool CanAddGasAccount => true;
		protected override bool IsSmartAccount => false;

		[Test]
		public async Task CanSeeAddGasAccountComponents()
		{
			var account = UserConfig.Accounts.First();

			Assert.IsTrue(
				Sut.AddGas?.TextContent.Equals("Add gas to your account and save 3% on your bills a year."));
			Assert.IsTrue(Sut.AddGasFlow?.TextContent.Equals("Add gas to my account"));
			Assert.IsTrue(
				Sut.AddGasFlow?.Href.Contains(
					$"NewContainedView?ElectricityAccountNumber={account.AccountNumber}"));
			Assert.IsTrue(Sut.AddGasFlow?.Href.Contains("NewContainedFlowType=AddGasAccount"));
			var page = await App.ClickOnElement(Sut.AddGasFlow);
			page.CurrentPageAs<CollectAccountConsumptionDetailsPage>();

		}
	}
}

using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AddGasAccount
{
	[TestFixture]
	internal class WhenInConfirmAddressPage_Test : WhenInCollectAccountConsumptionDetailsTest
	{
		[Test]
		public async Task TheViewShowsTheExpectedInformation()
		{
			Sut.InputFormValues(UserConfig, 12345);
			var sut = (await Sut.ClickOnElement(Sut.SubmitButton)).CurrentPageAs<ConfirmAddressPage>();
			Assert.IsNotNull(sut.ConfirmAddressButton);
			Assert.IsNotNull(sut.GPRNHeading);
			Assert.IsTrue(sut.GPRNHeading.TextContent.Contains(Sut.GPRN.Value));
			Assert.IsTrue(sut.Content.TextContent.Contains("The following address matches the GPRN you entered."));

			Assert.IsNotNull(sut.NotMyAddressButton);
			Assert.IsNotNull(sut.Address);

			Assert.AreEqual("No, this is not my new address. Try again.", sut.NotMyAddressButton.TextContent);
			Assert.IsNotNull(sut.NeedHelpContent);
			Assert.IsTrue(
				sut.NeedHelpContent.TextContent.Contains(
					"Need help? Contact an Electric Ireland agent on 1850 372 372"));
		}
	}
}
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation
{
	[TestFixture]
	class WhenMoveElectricityAndCloseGasManual : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.Manual;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;
	
		protected override MovingHouseType MovingHouseType => MovingHouseType.MoveElectricityAndCloseGas;

		protected override async Task<Step1InputMoveOutPage> ToStep1()
		{
			var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();
			return (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2)).CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public async Task Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(
				Sut.ConfirmationElectricityAccountNumber.TextContent, 
				"Expected to see electricty account number");

			Assert.IsNotNull(
				Sut.ConfirmationGasAccountNumber.TextContent,
				"Expected to see gas account number");

			Assert.AreEqual(
				"Thank you, your Electricity account has been moved to your new address, and your Gas account has been closed.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsTrue(Sut.YourNewSavingsWillBeAppliedNotice.TextContent.Trim().StartsWith("Your new savings will be applied within 24 hours."));

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

			Assert.AreEqual(
				"Your Electricity bill will be paid manually.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNull(Sut.YourGasAccountWillBePaidByNotice);

			Assert.AreEqual(
				ElectricityAndGasPricingUrl,
				Sut.ElectricityAndGasPricing.Attributes["href"].Value,
				"Should see correct link to pricing terms and conditions");

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}
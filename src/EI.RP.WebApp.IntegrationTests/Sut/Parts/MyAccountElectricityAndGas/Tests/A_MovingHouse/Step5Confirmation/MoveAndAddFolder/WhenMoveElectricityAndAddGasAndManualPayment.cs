using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.MoveAndAddFolder
{
	[TestFixture]
	class WhenMoveElectricityAndAddGasAndManualPayment : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.Manual;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType { get => MovingHouseType.MoveElectricityAndAddGas; }

		protected override async Task<Step1InputMoveOutPage> ToStep1()
		{
			var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();
			return (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2))
				.CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public async Task Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(Sut.ConfirmationElectricityAccountNumber);
			Assert.IsNull(Sut.ConfirmationGasAccountNumber);

			Assert.AreEqual(
				"Thank you, your accounts have been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.AreEqual(
				"We are creating your new account and in the next four weeks you'll receive your account number. Your new savings will be applied within 24 hours.",
				Sut.CreatingNewAccountsNotice.TextContent.Trim());

			Assert.AreEqual(
				ReadTermsandConditionsUrl,
				Sut.ReadTermsAndConditions.Attributes["href"].Value,
				"Should see correct link to pricing terms and conditions");

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

			Assert.AreEqual(
				"Your Electricity bill will be paid manually.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.AreEqual(
				"Your Gas bill will be paid manually.",
				Sut.YourGasAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}
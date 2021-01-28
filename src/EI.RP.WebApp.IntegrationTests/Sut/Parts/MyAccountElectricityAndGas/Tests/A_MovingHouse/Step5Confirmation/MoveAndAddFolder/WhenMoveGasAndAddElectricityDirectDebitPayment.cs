using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.MoveAndAddFolder
{
	[TestFixture]
	class WhenMoveGasAndAddElectricityDirectDebitPayment : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType { get => MovingHouseType.MoveGasAndAddElectricity; }

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
			Assert.IsNull(Sut.ConfirmationElectricityAccountNumber);
			Assert.IsNotNull(Sut.ConfirmationGasAccountNumber);

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

			Assert.IsNull(Sut.FreeElectricityAllowanceNotice, "Did not expect to see Free Electricty Allowance Notice");

			Assert.AreEqual(
				"Your Electricity bill will be paid by direct debit.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.AreEqual(
				"Your Gas bill will be paid by direct debit.",
				Sut.YourGasAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}
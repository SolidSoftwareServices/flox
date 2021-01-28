using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.MoveBoth
{
	[TestFixture]
	class WhenMoveElectricityAndMoveGasAndDirectDebitPayment : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType { get => MovingHouseType.MoveElectricityAndGas; }

		protected override async Task<Step1InputMoveOutPage> ToStep1()
		{
			var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();
			return (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
				.CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public void Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(Sut.ConfirmationElectricityAccountNumber);
			Assert.IsNotNull(Sut.ConfirmationGasAccountNumber);

			Assert.AreEqual(
				"Thank you, your accounts have been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsTrue(Sut.DirectDebitChangeConfirmation?.TextContent.Trim().Equals("You have changed your payment method to Direct Debit, your savings will be applied within 12 days."));
			
			Assert.IsTrue(Sut.ReadTermsAndConditions?.Href.Equals("https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing"));
			Assert.IsTrue(Sut.ReadTermsAndConditions?.TextContent.Trim().Equals("Read T&Cs."));

			Assert.IsTrue(Sut.DirectDebitBillMessage?.TextContent.Trim().Equals("Your first Direct Debit payment will be made 14 days after your next bill is issued. To update your Direct Debit details at any time go to"));
			Assert.IsTrue(Sut.BillAndPaymentLink?.TextContent.Trim().Equals("Payments"));

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

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

		[Test]
		public async Task CanNavigateToPaymentOptionsPage()
		{
			(await App.ClickOnElement(Sut.BillAndPaymentLink)).CurrentPageAs<ShowPaymentsHistoryPage>();
		}
	}
}
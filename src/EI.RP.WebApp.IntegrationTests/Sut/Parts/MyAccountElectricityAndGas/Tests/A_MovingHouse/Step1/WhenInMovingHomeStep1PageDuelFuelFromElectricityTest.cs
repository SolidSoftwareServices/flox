using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
	[TestFixture]
	class WhenInMovingHomeStep1PageDuelFuelFromElectricityTest : MyAccountCommonTests<Step1InputMoveOutPage>
	{
		protected override async Task TestScenarioArrangement()
		{
            UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices();
			UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
			UserConfig.Execute();

			await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
			await App.CurrentPageAs<AccountSelectionPage>()
				.SelectAccount(UserConfig.Accounts.Last().AccountNumber);
			await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
			await App.CurrentPageAs<Step0LandingPage>().ClickOnPopUpButton1();

			Sut = App.CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public async Task CanSeeComponents()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			var elecUserInfo = accountConfigurators[0].Model;
			var gasUserInfo = accountConfigurators[1].Model;

			var movingOutSelectedDateTime = elecUserInfo.ContractStartDate == DateTime.Now.AddDays(-1)
				? DateTime.Now.Date
				: DateTime.Now.AddDays(-1).Date;

			var elecDevicesMeterReading = accountConfigurators[0].Premise.ElectricityDevice().Registers.Single();

			Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);
			Assert.IsTrue(Sut.ElecMeterReadingTypeTitle.TextContent.Contains(elecDevicesMeterReading.MeterType));
            Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(elecUserInfo.PointReferenceNumber.ToString()));
			Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			var gasDevicesMeterReading = accountConfigurators[1].Premise.GasDevice().Registers.Single();
			Assert.IsTrue(Sut.GPRNHeader.TextContent.Contains(gasUserInfo.PointReferenceNumber.ToString()));

            AssertHasCorrectReadGeneralTermsLink(Sut);
		}

		private void AssertHasCorrectReadGeneralTermsLink(Step1InputMoveOutPage page)
		{
			Assert.IsNotNull(page.CheckBoxTerms);
			var links = page.GeneralTermsAndConditionsLinks.ToArray();
			var electricityLink = links[0];
			var gasLink = links[1];
			Assert.IsTrue(
				electricityLink.Attributes["href"].Value.ToLowerInvariant() ==
				"https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity",
				"expected to see link to terms and conditions for electricity");
			Assert.IsTrue(
				gasLink.Attributes["href"].Value.ToLowerInvariant() ==
				"https://electricireland.ie/residential/helpful-links/terms-conditions/residential-gas",
				"expected to see link to terms and conditions for gas");
		}

		[Test]
		public async Task CanSeeValidationErrorMessage()
		{
			Sut.IncomingOccupantYesCheckedBox.IsChecked = true;
			Sut = (await Sut.ClickOnElement(Sut.GetNextPRNButton())).CurrentPageAs<Step1InputMoveOutPage>();
			Assert.AreEqual("Please enter a valid meter reading", Sut.OneElectricityMeterReadingError.TextContent);
			Assert.AreEqual("Please enter a valid meter reading", Sut.GasMeterReadingError.TextContent);
			Assert.AreEqual("Please provide the name of the incoming occupier", Sut.LettingNameError.TextContent);
			Assert.AreEqual("You must enter a valid phone number", Sut.LettingPhoneNumberError.TextContent);
			Assert.AreEqual("Please accept the occupier details.", Sut.OccupierDetailsAcceptedError.TextContent);
		}

		[Test]
		public async Task ThrowsValidations_Fields_Empty()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();

			var elecUserInfo = accountConfigurators[0].Model;
			var gasUserInfo = accountConfigurators[1].Model;

			Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(elecUserInfo.PointReferenceNumber.ToString()));
			Assert.IsTrue(Sut.GPRNHeader.TextContent.Contains(gasUserInfo.PointReferenceNumber.ToString()));
			var sut = (await Sut.ClickOnElement(Sut.GetNextPRNButton())).CurrentPageAs<Step1InputMoveOutPage>();
			Assert.IsTrue(sut.MPRNHeader.TextContent.Contains(elecUserInfo.PointReferenceNumber.ToString()));
			Assert.IsTrue(sut.GPRNHeader.TextContent.Contains(gasUserInfo.PointReferenceNumber.ToString()));
			Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
		}

		[Test]
		public async Task WhenClickSubmitButton()
		{
			Assert.IsNotNull(Sut.GetNextPRNButton());
		}
	}
}
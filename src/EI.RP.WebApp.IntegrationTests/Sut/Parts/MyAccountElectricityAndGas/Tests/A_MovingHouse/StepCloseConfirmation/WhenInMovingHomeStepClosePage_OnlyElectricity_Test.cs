﻿using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.StepCloseConfirmation
{
	[TestFixture]
	class WhenInMovingHomeStepClosePage_OnlyElectricity_Test : MyAccountCommonTests<StepCloseAccountPageConfirmationPage>
	{
		protected override async Task TestScenarioArrangement()
		{

			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(configureDefaultDevice:false).WithElectricity24HrsDevices();
			UserConfig.Execute();
			var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

			var stepClosePage = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton3))
				.CurrentPageAs<StepCloseAccountPage>();

			stepClosePage = FillRequiredFields(stepClosePage);
			Sut = (await stepClosePage.ClickOnElement(stepClosePage.GetCloseButton()))
				.CurrentPageAs<StepCloseAccountPageConfirmationPage>();
		}
		[Test]
		public async Task CanSeeComponents()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			Assert.IsTrue(Sut.GetElectricityAccountInfo().Text().Contains("Electricity"));
			Assert.IsTrue(Sut.GetElectricityAccountInfo().Text().Contains(accountConfigurators[0].Model.AccountNumber));
			Assert.AreEqual($"Your Electricity bill will be paid manually.", Sut.GetElectricityPaymentInfo().TextContent);
		}

		private StepCloseAccountPage FillRequiredFields(StepCloseAccountPage stepClosePage)
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			var elecDevicesMeterReading = accountConfigurators[0].Premise.ElectricityDevice().Registers.Single();

			stepClosePage.IsROIFieldRequired.Value = "true";
			stepClosePage.GetRoiHouseNumberInputElement().Value = "HouseNumber";
			stepClosePage.GetRoiStreetInputElement().Value = "Street";
			stepClosePage.GetRoiTownInputElement().Value = "Town";
			stepClosePage.GetRoiRoiCountyDropDownElementOption().OuterHtml = "<option selected=\"selected\" value=\"CK\">Cork</option>";

			stepClosePage.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			stepClosePage.CheckBoxDetails.IsChecked = true;
			stepClosePage.CheckBoxTerms.IsChecked = true;

			return stepClosePage;
		}

		[Test]
		public async Task CanGoToAccountsPageAfterClickBackToAccounts()
		{
			var backToMyAccountsLink = Sut.BackToMyAccountsLink;
			Assert.NotNull(backToMyAccountsLink);

			var accountsPage = (await Sut.ClickOnElement(backToMyAccountsLink))
							.CurrentPageAs<AccountSelectionPage>();

			Assert.NotNull(accountsPage);
		}

	}
}

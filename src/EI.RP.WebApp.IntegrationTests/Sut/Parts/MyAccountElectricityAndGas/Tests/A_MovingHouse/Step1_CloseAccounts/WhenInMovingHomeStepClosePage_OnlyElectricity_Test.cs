using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1_CloseAccounts
{
    [TestFixture]
    class WhenInMovingHomeStepClosePage_OnlyElectricity_Test : MyAccountCommonTests<StepCloseAccountPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            ElectricityConfigurator= UserConfig.AddElectricityAccount(configureDefaultDevice:false).WithElectricity24HrsDevices();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

			Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton3))
				.CurrentPageAs<StepCloseAccountPage>();
		}
        private ElectricityAccountConfigurator ElectricityConfigurator { get; set; }

		void AssertHasCorrectReadGeneralTermsLink(StepCloseAccountPage page)
		{
			Assert.IsNotNull(page.CheckBoxTerms);
			var electricityLink = page.GeneralTermsAndConditionsLinks.Single();
			Assert.IsTrue(electricityLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity", "expected to see link to terms and conditions for electricity");
		}

		[Test]
        public async Task CanSeeComponents()
        {
	        var elecUserInfo = ElectricityConfigurator.Model;

	        var movingOutSelectedDateTime = elecUserInfo.ContractStartDate == DateTime.Now.AddDays(-1)
		        ? DateTime.Now.Date
		        : DateTime.Now.AddDays(-1).Date;
	        Assert.IsTrue(Sut.DatePickerTitle.TextContent.Contains($"Date of move out"));
	        Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);

	        var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();

	        Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);
	        Assert.IsTrue(Sut.ElecMeterReadingTypeTitle.TextContent.Contains(elecDevicesMeterReading.MeterType));
	        Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(elecUserInfo.PointReferenceNumber.ToString()));
	        Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
	        Assert.AreEqual("Your Forwarding Address", Sut.ForwardingAddressHeader.TextContent);

	        AssertHasCorrectReadGeneralTermsLink(Sut);
        }

		[Test]
		public async Task WhenClickSubmitButtonWithoutMandatoryFields()
		{
			Sut.IsROIFieldRequired.Value = "true";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("Please enter house number", Sut.RoiHouseNumberError.TextContent);
			Assert.AreEqual("Please enter street", Sut.RoiStreetError.TextContent);
			Assert.AreEqual("You must provide a town", Sut.RoiTownError.TextContent);
			Assert.AreEqual("You must select a county or state", Sut.RoiCountyError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();

			Sut.IsPOBoxFieldRequired.Value = "true";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("You must insert a PO box number", Sut.PoPOBoxNumberError.TextContent);
			Assert.AreEqual("You must insert a post code", Sut.PoPOBoxPostCodeError.TextContent);
			Assert.AreEqual("You must select a county or state", Sut.PoDistrictError.TextContent);
			Assert.AreEqual("You must select a country", Sut.PoCountryError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();

			Sut.IsNIFieldRequired.Value = "true";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsROIFieldRequired.Value = "false";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("You must select a country", Sut.NiCountryError.TextContent);
			Assert.AreEqual("Please enter house number", Sut.NiHouseNumberError.TextContent);
			Assert.AreEqual("Please enter street", Sut.NiStreetError.TextContent);
			Assert.AreEqual("You must provide a town", Sut.NiTownError.TextContent);
			Assert.AreEqual("You must insert a post code", Sut.NiPostCodeError.TextContent);
			Assert.AreEqual("You must select a county or state", Sut.NiDistrictError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();
			Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			AssertHasCorrectReadGeneralTermsLink(Sut);
		}

		[Test]
		public async Task WhenSubmittingRoiAddressExecutesDomainCommand()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();

			Sut.IsROIFieldRequired.Value = "true";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.GetRoiHouseNumberInputElement().Value = "HouseNumber";
			Sut.GetRoiStreetInputElement().Value = "Street";
			Sut.GetRoiTownInputElement().Value = "Town";
			Sut.GetRoiRoiCountyDropDownElementOption().OuterHtml = "<option selected=\"selected\" value=\"CK\">Cork</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;

			var addressInfo = new AddressInfo();
			addressInfo.PostalCode = string.Empty;
			addressInfo.Street = Sut.GetRoiStreetInputElement().Value.ToUpper();
			addressInfo.City = Sut.GetRoiTownInputElement().Value.ToUpper();
			addressInfo.AddressLine1 = string.Empty;
			addressInfo.AddressLine5 = string.Empty;
			addressInfo.AddressLine4 = string.Empty;
			addressInfo.AddressLine2 = string.Empty;
			addressInfo.HouseNo = Sut.GetRoiHouseNumberInputElement().Value.ToUpper();
			addressInfo.Region = "CK";
			addressInfo.AddressType = AddressType.RepublicOfIreland;
			var cmd = BuildCommand(addressInfo);
			(await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPageConfirmationPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

	    private CloseAccountsCommand BuildCommand(AddressInfo addressInfo)
	    {
		    var cmd = new CloseAccountsCommand(ClientAccountType.Electricity, addressInfo,
			    Convert.ToDateTime(Sut.MoveOutDatePicker.Value),
			    ElectricityConfigurator.ToElectricityMeterReading(Sut), moveOutIncommingOccupantInfo: new MoveOutIncommingOccupantInfo());
		    return cmd;
	    }


		[Test]
		public async Task WhenSubmittingPoAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();

			Sut.IsPOBoxFieldRequired.Value = "true";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";

			Sut.GetPOBoxNumberTextBox().Value = "PoBoxNumber";
			Sut.GetPOBoxPostCodeTextBox().Value = "PostCode";
			Sut.GetPoDistrictTextBox().Value = "District";
			Sut.PostalAddressCountryOption().OuterHtml = "<option selected=\"selected\" value=\"AF\">Afghanistan</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;

			var addressInfo = new AddressInfo();
			addressInfo.POBoxPostalCode = Sut.GetPOBoxPostCodeTextBox().Value.ToUpper();
			addressInfo.POBox = Sut.GetPOBoxNumberTextBox().Value.ToUpper();
			addressInfo.District = Sut.GetPoDistrictTextBox().Value.ToUpper();
			addressInfo.CountryID = "AF";
			addressInfo.AddressType = AddressType.PO;
			var cmd = BuildCommand(addressInfo);
			(await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPageConfirmationPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

		}
		[Test]
		public async Task WhenSubmittingNiAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();

			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "true";
			Sut.GetNiHouseNumber().Value = "HouseNumber";
			Sut.GetNiStreet().Value = "Street";
			Sut.GetNiTown().Value = "Town";
			Sut.GetNiPostCode().Value = "PostCode";
			Sut.GetNiDistrict().Value = "District";
			Sut.GetNiTown().Value = "Town";
			Sut.GetNiCountryOption().OuterHtml = "<option selected=\"selected\" value=\"AF\">Afghanistan</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;

			var addressInfo = new AddressInfo();
			addressInfo.Street = Sut.GetNiStreet().Value.ToUpper();
			addressInfo.City = Sut.GetNiTown().Value.ToUpper();
			addressInfo.PostalCode = Sut.GetNiPostCode().Value.ToUpper();
			addressInfo.CountryID = "AF";
			addressInfo.AddressLine4 = string.Empty;
			addressInfo.AddressLine5 = string.Empty;
			addressInfo.AddressLine1 = string.Empty;
			addressInfo.AddressLine2 = string.Empty;
			addressInfo.HouseNo = Sut.GetNiHouseNumber().Value.ToUpper();
			addressInfo.District = Sut.GetNiDistrict().Value.ToUpper();
			addressInfo.AddressType = AddressType.NI;
			var cmd = BuildCommand(addressInfo);
			(await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPageConfirmationPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

		}
	}
}

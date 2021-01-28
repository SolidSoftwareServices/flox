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
    class WhenInMovingHomeStepClosePageOnlyElectricityWithDayAndNightMeter : MyAccountCommonTests<StepCloseAccountPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            ElectricityConfigurator=UserConfig.AddElectricityAccount(configureDefaultDevice:false).WithElectricityDayAndNightDevices();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

			Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton3))
				.CurrentPageAs<StepCloseAccountPage>();
		}

        private ElectricityAccountConfigurator ElectricityConfigurator { get; set; }

		[Test]
		public async Task CanSeeComponents()
		{
			var userInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			var movingOutSelectedDateTime = userInfo.ContractStartDate == DateTime.Now.AddDays(-1)
				? DateTime.Now.Date
				: DateTime.Now.AddDays(-1).Date;
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.Single();

			var deviceRegisterInfos = accountConfigurators.Premise.Devices.Single().Registers.ToArray();
			var elecDayDevicesMeterReading = deviceRegisterInfos.Single(x=>x.MeterType==MeterType.ElectricityDay);
			var elecNightDevicesMeterReading = deviceRegisterInfos.Single(x => x.MeterType == MeterType.ElectricityNight);

			Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);

            Assert.AreEqual(true, Sut.IncomingOccupantNoCheckedBox.IsChecked);
			Assert.AreEqual(false, Sut.IncomingOccupantYesCheckedBox.IsChecked);
			Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));
			Assert.AreEqual("Your Forwarding Address", Sut.ForwardingAddressHeader.TextContent);
		}
		[Test]
		public async Task CanSeeValidationErrorMessage()
		{
			Sut.IncomingOccupantYesCheckedBox.IsChecked = true;
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("Please enter a valid meter reading", Sut.DayElectricityMeterError.TextContent);
			Assert.AreEqual("Please enter a valid meter reading", Sut.NightElectricityMeterError.TextContent);
			Assert.AreEqual("Please provide the name of the incoming occupier", Sut.LettingNameError.TextContent);
			Assert.AreEqual("You must enter a valid phone number", Sut.LettingPhoneNumberError.TextContent);
			Assert.AreEqual("Please accept the occupier details.", Sut.OccupierDetailsAcceptedError.TextContent);
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
		}

		[Test]
		public async Task WhenSubmittingRoiAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers;
			var elecDayDevicesMeterReading = elecDevicesMeterReading.ToArray()[0];
			var elecNightDevicesMeterReading = elecDevicesMeterReading.ToArray()[1];
			Sut.IsROIFieldRequired.Value = "true";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.GetRoiHouseNumberInputElement().Value = "HouseNumber";
			Sut.GetRoiStreetInputElement().Value = "Street";
			Sut.GetRoiTownInputElement().Value = "Town";
			Sut.GetRoiRoiCountyDropDownElementOption().OuterHtml = "<option selected=\"selected\" value=\"CK\">Cork</option>";
			Sut.GetElectricityReadingInput(elecDayDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetElectricityReadingInput(elecNightDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();

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
		[Test]
		public async Task WhenSubmittingPoAddressExecutesDomainCommand()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers;
			var elecDayDevicesMeterReading = elecDevicesMeterReading.ToArray()[0];
			var elecNightDevicesMeterReading = elecDevicesMeterReading.ToArray()[1];
			Sut.IsPOBoxFieldRequired.Value = "true";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";

			Sut.GetPOBoxNumberTextBox().Value = "PoBoxNumber";
			Sut.GetPOBoxPostCodeTextBox().Value = "PostCode";
			Sut.GetPoDistrictTextBox().Value = "District";
			Sut.PostalAddressCountryOption().OuterHtml = "<option selected=\"selected\" value=\"AF\">Afghanistan</option>";
			Sut.GetElectricityReadingInput(elecDayDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetElectricityReadingInput(elecNightDevicesMeterReading.MeterNumber).Value = "123";
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
	    private CloseAccountsCommand BuildCommand(AddressInfo addressInfo)
	    {
		    var cmd = new CloseAccountsCommand(ClientAccountType.Electricity, addressInfo,
			    Convert.ToDateTime(Sut.MoveOutDatePicker.Value),
			    ElectricityConfigurator.ToElectricityMeterReading(Sut),moveOutIncommingOccupantInfo:new MoveOutIncommingOccupantInfo());
		    return cmd;
	    }

		[Test]
		public async Task WhenSubmittingNiAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.ToArray();
			var elecDayDevicesMeterReading = elecDevicesMeterReading[0];
			var elecNightDevicesMeterReading = elecDevicesMeterReading[1];
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
			Sut.GetElectricityReadingInput(elecDayDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetElectricityReadingInput(elecNightDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;

			var addressInfo = new AddressInfo();
			addressInfo.Street = Sut.GetNiStreet().Value.ToUpper();
			addressInfo.City = Sut.GetNiTown().Value.ToUpper();
			addressInfo.PostalCode = Sut.GetNiPostCode().Value.ToUpper();
			addressInfo.CountryID = "AF";
			addressInfo.AddressType=AddressType.NI;
			addressInfo.AddressLine4 = string.Empty;
			addressInfo.AddressLine5 = string.Empty;
			addressInfo.AddressLine1 = string.Empty;
			addressInfo.AddressLine2 = string.Empty;
			addressInfo.HouseNo = Sut.GetNiHouseNumber().Value.ToUpper();
			addressInfo.District = Sut.GetNiDistrict().Value.ToUpper();

			var cmd = BuildCommand(addressInfo);
			(await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPageConfirmationPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

		}
	}
}

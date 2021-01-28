using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1_CloseAccounts
{
	[TestFixture]
	internal class WhenInMovingHomeStepClosePage_DuelFuelFromGasTest : MyAccountCommonTests<StepCloseAccountPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			ElectricityConfigurator = UserConfig.AddElectricityAccount(configureDefaultDevice: false)
				.WithElectricity24HrsDevices();
			GasConfigurator = UserConfig.AddGasAccount(paymentType: PaymentMethodType.DirectDebit,
				duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
			UserConfig.Execute();

			var gasAccountNumber = UserConfig.Accounts.FirstOrDefault(x => x.IsGasAccount()).AccountNumber;

			await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
			await App.CurrentPageAs<AccountSelectionPage>().SelectAccount(gasAccountNumber);
			await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome(gasAccountNumber);
			await App.CurrentPageAs<Step0LandingPage>().ClickOnPopUpButton3();

			Sut = App.CurrentPageAs<StepCloseAccountPage>();
		}

		private ElectricityAccountConfigurator ElectricityConfigurator { get; set; }
		private GasAccountConfigurator GasConfigurator { get; set; }

		private CloseAccountsCommand BuildCommand(AddressInfo addressInfo)
		{
			var cmd = new CloseAccountsCommand(ClientAccountType.Gas, addressInfo,
				Convert.ToDateTime(Sut.MoveOutDatePicker.Value),
				ElectricityConfigurator.ToElectricityMeterReading(Sut),
				GasConfigurator.ToGasMeterReading(Sut), new MoveOutIncommingOccupantInfo());
			return cmd;
		}

		[Test]
		public async Task CanSeeComponents()
		{
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
			var elecUserInfo = ElectricityConfigurator.Model;
			var gasUserInfo = GasConfigurator.Model;

			var movingOutSelectedDateTime = elecUserInfo.ContractStartDate == DateTime.Now.AddDays(-1)
				? DateTime.Now.Date
				: DateTime.Now.AddDays(-1).Date;
			Assert.IsTrue(Sut.DatePickerTitle.TextContent.Contains($"Date of move out"));
			Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);

			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();

			Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);
			Assert.IsTrue(Sut.ElecMeterReadingTypeTitle.TextContent.Contains(elecDevicesMeterReading.MeterType));
			Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(elecUserInfo.PointReferenceNumber.ToString()));

			var gasDevicesMeterReading = GasConfigurator.Premise.GasDevice().Registers.Single();
			Assert.IsTrue(Sut.GPRNHeader.TextContent.Contains(gasUserInfo.PointReferenceNumber.ToString()));
			Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			Assert.AreEqual("Your Forwarding Address", Sut.ForwardingAddressHeader.TextContent);

			AssertHasCorrectReadGeneralTermsLink(Sut);

			void AssertHasCorrectReadGeneralTermsLink(StepCloseAccountPage page)
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
		}

		[Test]
		public async Task CanSeeInvalidError()
		{
			Sut.IsROIFieldRequired.Value = "true";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.GetRoiHouseNumberInputElement().Value = "$$%%";
			Sut.GetRoiStreetInputElement().Value = "$$%%";
			Sut.GetRoiTownInputElement().Value = "$$%%";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("Please enter a valid house number", Sut.RoiHouseNumberError.TextContent);
			Assert.AreEqual("Please enter a valid street", Sut.RoiStreetError.TextContent);
			Assert.AreEqual("Please enter a valid town", Sut.RoiTownError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();

			Sut.IsPOBoxFieldRequired.Value = "true";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.GetPOBoxNumberTextBox().Value = "$$%%";
			Sut.GetPOBoxPostCodeTextBox().Value = "$$%%";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("You must enter a valid po box number", Sut.PoPOBoxNumberError.TextContent);
			Assert.AreEqual("Please enter a valid po box post code", Sut.PoPOBoxPostCodeError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();

			Sut.IsNIFieldRequired.Value = "true";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.GetNiHouseNumber().Value = "$$%%";
			Sut.GetNiStreet().Value = "$$%%";
			Sut.GetNiTown().Value = "$$%%";
			Sut.GetNiPostCode().Value = "$$%%";
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("Please enter a valid house number", Sut.NiHouseNumberError.TextContent);
			Assert.AreEqual("Please enter a valid street", Sut.NiStreetError.TextContent);
			Assert.AreEqual("Please enter a valid town", Sut.NiTownError.TextContent);
			Assert.AreEqual("Please enter a valid post code", Sut.NiPostCodeError.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CloseAccountsCommand>();
		}

		[Test]
		public async Task CanSeeNIAddressFields()
		{
			Assert.AreEqual("Non-Republic of Ireland postal address", Sut.GetNiHeader().TextContent);
			Assert.AreEqual("Country *", Sut.GetLabelNICountryList().TextContent.Trim());
			Assert.IsTrue(Sut.GetNiCountry().TextContent.Contains("Select Country"));
			Assert.AreEqual("Address Line 1", Sut.GetLabelNiAddressline1().TextContent.Trim());
			Assert.AreEqual("Address Line 1", Sut.GetniAddressline1().Placeholder);
			Assert.AreEqual("House Number *", Sut.GetLabelNiHouseNumber().TextContent.Trim());
			Assert.AreEqual("House Number", Sut.GetNiHouseNumber().Placeholder);
			Assert.AreEqual("Street Name *", Sut.GetLabelNiStreet().TextContent.Trim());
			Assert.AreEqual("Street", Sut.GetNiStreet().Placeholder);
			Assert.AreEqual("Address Line 2", Sut.GetLabelNiAddressline2().TextContent.Trim());
			Assert.AreEqual("Address Line 2", Sut.GetNiAddressline2().Placeholder);
			Assert.AreEqual("City / Town *", Sut.GetLabelNiTown().TextContent.Trim());
			Assert.AreEqual("City / Town", Sut.GetNiTown().Placeholder);
			Assert.AreEqual("Post Code *", Sut.GetLabelNiPostCode().TextContent.Trim());
			Assert.AreEqual("Post Code", Sut.GetNiPostCode().Placeholder);
			Assert.AreEqual("County / State *", Sut.GetLabelNiDistrict().TextContent.Trim());
			Assert.AreEqual("County / State", Sut.GetNiDistrict().Placeholder);
		}

		[Test]
		public async Task CanSeePoAddressFields()
		{
			Assert.AreEqual("PO Box Number *", Sut.GetLabelPOBoxNumber().TextContent.Trim());
			Assert.AreEqual("PO Box Number", Sut.GetPOBoxNumberTextBox().Placeholder);
			Assert.AreEqual("PO Box Post Code *", Sut.GetLabelPOBoxPostCode().TextContent.Trim());
			Assert.AreEqual("PO Box Post Code", Sut.GetPOBoxPostCodeTextBox().Placeholder);
			Assert.AreEqual("PO Box County/State *", Sut.GetLabelPoDistrict().TextContent.Trim());
			Assert.AreEqual("PO Box County/State", Sut.GetPoDistrictTextBox().Placeholder);
			Assert.AreEqual("PO Box Country *", Sut.GetLabelPoBoxCountry().TextContent.Trim());
			Assert.IsTrue(Sut.GetPoCountry().TextContent.Contains("Select Country"));
		}

		[Test]
		public async Task CanSeeRoiAddressFields()
		{
			Assert.AreEqual("Republic of Ireland postal address", Sut.GetRoiAddressHeadingElement().TextContent);
			Assert.AreEqual("Address Line 1", Sut.GetRoiAddressLine1Element().TextContent);
			Assert.AreEqual("Address Line 1", Sut.GetRoiAddressLine1InputElement().Placeholder);
			Assert.AreEqual("House Number *", Sut.GetRoiHouseNumberElement().TextContent);
			Assert.AreEqual("House Number", Sut.GetRoiHouseNumberInputElement().Placeholder);
			Assert.AreEqual("Street Name *", Sut.GetRoiStreetLabelElement().TextContent);
			Assert.AreEqual("Street", Sut.GetRoiStreetInputElement().Placeholder);
			Assert.AreEqual("Address Line 2", Sut.GetRoiAddressLine2Element().TextContent);
			Assert.AreEqual("Address Line 2", Sut.GetRoiAddressLine2InputElement().Placeholder);
			Assert.AreEqual("Town *", Sut.GetRoiTownElement().TextContent);
			Assert.AreEqual("Town", Sut.GetRoiTownInputElement().Placeholder);
			Assert.AreEqual("County *", Sut.GetRoiRoiCountyElement().TextContent);
			Assert.IsTrue(Sut.GetRoiRoiCountyDropDownElement().TextContent.Contains("Select County"));
			Assert.AreEqual("Post Code", Sut.GetRoiPostCodeElement().TextContent);
			Assert.AreEqual("Post Code", Sut.GetRoiPostCodeInputElement().Placeholder);
		}

		[Test]
		public async Task CanSeeValidationErrorMessage()
		{
			Sut.IncomingOccupantYesCheckedBox.IsChecked = true;
			Sut = (await Sut.ClickOnElement(Sut.GetCloseButton())).CurrentPageAs<StepCloseAccountPage>();
			Assert.AreEqual("Please enter a valid meter reading", Sut.OneElectricityMeterReadingError.TextContent);
			Assert.AreEqual("Please enter a valid meter reading", Sut.GasMeterReadingError.TextContent);
			Assert.AreEqual("Please provide the name of the incoming occupier", Sut.LettingNameError.TextContent);
			Assert.AreEqual("You must enter a valid phone number", Sut.LettingPhoneNumberError.TextContent);
			Assert.AreEqual("Please accept the occupier details.", Sut.OccupierDetailsAcceptedError.TextContent);
			Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
			Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains(
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
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
		public async Task WhenSubmittingNiAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();
			var gasDevicesMeterReading = GasConfigurator.Premise.GasDevice().Registers.Single();

			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "true";
			Sut.GetNiHouseNumber().Value = "HouseNumber";
			Sut.GetNiStreet().Value = "Street";
			Sut.GetNiTown().Value = "Town";
			Sut.GetNiPostCode().Value = "PostCode";
			Sut.GetNiDistrict().Value = "District";
			Sut.GetNiCountryOption().OuterHtml = "<option selected=\"selected\" value=\"AF\">Afghanistan</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value = "123";
			Sut.CheckBoxDetails.IsChecked = true;
			Sut.CheckBoxTerms.IsChecked = true;

			var addressInfo = new AddressInfo();
			addressInfo.Street = Sut.GetNiStreet().Value.ToUpper();
			addressInfo.City = Sut.GetNiTown().Value.ToUpper();
			addressInfo.PostalCode = Sut.GetNiPostCode().Value.ToUpper();
			addressInfo.CountryID = "AF";
			addressInfo.AddressType = AddressType.NI;

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

		[Test]
		public async Task WhenSubmittingPoAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();
			var gasDevicesMeterReading = GasConfigurator.Premise.GasDevice().Registers.Single();

			Sut.IsPOBoxFieldRequired.Value = "true";
			Sut.IsROIFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";

			Sut.GetPOBoxNumberTextBox().Value = "PoBoxNumber";
			Sut.GetPOBoxPostCodeTextBox().Value = "PostCode";
			Sut.GetPoDistrictTextBox().Value = "District";
			Sut.PostalAddressCountryOption().OuterHtml =
				"<option selected=\"selected\" value=\"AF\">Afghanistan</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value = "123";
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
		public async Task WhenSubmittingRoiAddressExecutesDomainCommand()
		{
			var elecDevicesMeterReading = ElectricityConfigurator.Premise.ElectricityDevice().Registers.Single();
			var gasDevicesMeterReading = GasConfigurator.Premise.GasDevice().Registers.Single();

			Sut.IsROIFieldRequired.Value = "true";
			Sut.IsPOBoxFieldRequired.Value = "false";
			Sut.IsNIFieldRequired.Value = "false";
			Sut.GetRoiHouseNumberInputElement().Value = "HouseNumber";
			Sut.GetRoiStreetInputElement().Value = "Street";
			Sut.GetRoiTownInputElement().Value = "Town";
			Sut.GetRoiRoiCountyDropDownElementOption().OuterHtml =
				"<option selected=\"selected\" value=\"CK\">Cork</option>";
			Sut.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
			Sut.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value = "123";
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
	}
}
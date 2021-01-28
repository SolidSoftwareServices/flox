using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step3InputMoveInPropertyDetailsPage : MyAccountElectricityAndGasPage
	{
		public Step3InputMoveInPropertyDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='mimo-3']") as IHtmlElement;
		public IHtmlInputElement MoveOutDatePicker => Document.QuerySelector("#move_out") as IHtmlInputElement;

		public IHtmlHeadingElement NewPropertyDetailsHeader =>
			Document.QuerySelector("[data-testid='mimo-3-title']") as IHtmlHeadingElement;

		public IHtmlAnchorElement CancelButton => Document.QuerySelector("#btnCancel") as IHtmlAnchorElement;
		public IHtmlAnchorElement CancelMovePage => Document.QuerySelector("#cancelMoveHouse") as IHtmlAnchorElement;

		public IHtmlLabelElement MoveInDatePickerLabel =>
			Document.QuerySelector("#moveOutDate > label") as IHtmlLabelElement;

		public IHtmlLabelElement ContactNumberLabel => Document.QuerySelector("#lblContactNumber") as IHtmlLabelElement;

		public IHtmlLabelElement ElectricityMeterTypeLabel =>
			Document.QuerySelector("#lblElecMeterType") as IHtmlLabelElement;

		public IHtmlLabelElement GasMeterTypeLabel => Document.QuerySelector("#lblGasMeterType") as IHtmlLabelElement;
		public IHtmlInputElement ContactNumber => Document.QuerySelector("#step3ContactNumber") as IHtmlInputElement;

		public IHtmlSpanElement GasMeterReadingError =>
			(IHtmlSpanElement) Document.QuerySelector("#gasMeterReadingError");

		public IHtmlSpanElement OneElectricityMeterReadingError =>
			(IHtmlSpanElement) Document.QuerySelector("#oneElectricityMeterError");

		public IHtmlSpanElement ContactNumberError => (IHtmlSpanElement) Document.QuerySelector("#contactNumberError");

		public IHtmlInputElement CheckBoxDetails =>
			Document.QuerySelector("#details") as IHtmlInputElement;

		public IHtmlInputElement CheckBoxTerms =>
			Document.QuerySelector("#terms") as IHtmlInputElement;

		public IEnumerable<IHtmlElement> GeneralTermsAndConditionsLinks =>
			Document.QuerySelectorAll<IHtmlElement>("#termsAndConditions a");

		public IHtmlHeadingElement ElectricityMeterReadingsHeader =>
			Document.QuerySelector("#electricityMeterReadingHeader") as IHtmlHeadingElement;

		public IHtmlDivElement MprnHeader => Document.QuerySelector("#MprnHeader") as IHtmlDivElement;
		public IHtmlDivElement GprnHeader => Document.QuerySelector("#GprnHeader") as IHtmlDivElement;

		public IHtmlHeadingElement GasMeterReadingsHeader =>
			Document.QuerySelector("#gasMeterReadingHeader") as IHtmlHeadingElement;

		public IHtmlButtonElement NextPaymentOptions =>
			Document.QuerySelector("#btnNextPaymentOptions") as IHtmlButtonElement;

		public IHtmlParagraphElement GasMeterReadingDescription =>
			Document.QuerySelector("#pGasMeterReadingDescription") as IHtmlParagraphElement;

		public IHtmlParagraphElement ElectricityMeterReadingDescription =>
			Document.QuerySelector("#pElectricityMeterReadingDescription") as IHtmlParagraphElement;

		public IHtmlElement HasReadGeneralTermsAndConditionsSection =>
			Document.QuerySelector("#termsAndConditions") as IHtmlElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("3. New Property Details | Moving House"));
			}

			return isInPage;
		}

		public IHtmlButtonElement GetNextStep3Button()
		{
			return Document.QuerySelector("#btnNextPRN") as IHtmlButtonElement;
		}

		public IHtmlInputElement GetElectricityReadingInput(string meterNumber)
		{
			return Document.QuerySelector($"#txt_elec_{meterNumber}") as IHtmlInputElement;
		}

		public IHtmlInputElement GetGasReadingInput(string meterNumber)
		{
			return Document.QuerySelector("[id^=txt_gas_0_]") as IHtmlInputElement;
		}

		public Step3InputMoveInPropertyDetailsPage InputFormValues(AppUserConfigurator userConfig,
			string readingValue = null)
		{
			MoveOutDatePicker.Value = DateTime.Today.Subtract(TimeSpan.FromDays(2.0)).ToShortDateString();
			if (readingValue == null)
				readingValue = "1234";

			var electricityAccount = userConfig.ElectricityAccount();
			var gasAccount = userConfig.GasAccount();

			ContactNumber.Value = "0879876543";

			var deviceRegisterInfo = GetElectricityDeviceRegisterInfo();
			if (deviceRegisterInfo != null)
			{
				var input = GetElectricityReadingInput(deviceRegisterInfo.MeterNumber);
				if (input != null) input.Value = readingValue;
			}

			deviceRegisterInfo = GetGasDeviceRegisterInfo();
			if (deviceRegisterInfo != null)
			{
				var input = GetGasReadingInput(deviceRegisterInfo.MeterNumber);
				if (input != null) input.Value = readingValue;
			}


			CheckBoxDetails.IsChecked = true;
			CheckBoxTerms.IsChecked = true;
			return this;

			DeviceRegisterInfo GetElectricityDeviceRegisterInfo()
			{
				return (electricityAccount?.NewPremise.PremiseInfo.Installations
					        .SelectMany(x => x.Devices.SelectMany(y => y.Registers))
				        ?? gasAccount.NewDuelFuelAccountConfigurator?.Devices.SelectMany(x => x.Registers)
					)?.Single(x => x.MeterType.IsElectricity());
			}

			DeviceRegisterInfo GetGasDeviceRegisterInfo()
			{
				return (gasAccount?.NewPremise.PremiseInfo.Installations
					        .SelectMany(x => x.Devices.SelectMany(y => y.Registers))
				        ?? electricityAccount.NewDuelFuelAccountConfigurator?.Devices.SelectMany(x => x.Registers))
					?.Single(x => x.MeterType.IsGas());
			}
		}


		public void AssertInitialViewComponents(AppUserConfigurator userConfig, bool isClosingDualGas = false)
		{
			Assert.IsTrue(NewPropertyDetailsHeader.TextContent.Contains("New Property Details"));
			Assert.IsTrue(MoveInDatePickerLabel.TextContent.Contains($"Date of move in"));
			Assert.AreEqual("Contact Phone Number *", ContactNumberLabel.TextContent.Trim());
			AssertInitialElectricityViewComponents(userConfig);
			AssertInitialGasViewComponents(userConfig, isClosingDualGas);
			AssertHasCorrectReadGeneralTermsLink();
		}

		public void AssertHasCorrectReadGeneralTermsLink()
		{
			Assert.IsNotNull(CheckBoxTerms);
			var links = GeneralTermsAndConditionsLinks.ToArray();
			if (MprnHeader != null)
			{
				var electricityLink = links[0];
				Assert.IsTrue(
					electricityLink.Attributes["href"].Value.ToLowerInvariant() ==
					"https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity",
					"expected to see link to terms and conditions for electricity");
			}

			if (GprnHeader != null)
			{
				var gasLink = links[MprnHeader != null ? 1 : 0];
				Assert.IsTrue(
					gasLink.Attributes["href"].Value.ToLowerInvariant() ==
					"https://electricireland.ie/residential/helpful-links/terms-conditions/residential-gas",
					"expected to see link to terms and conditions for gas");
			}
		}

		public void AssertInitialGasViewComponents(AppUserConfigurator userConfig, bool isClosingDualGas)
		{
			var gasAccount = userConfig.GasAccount();
			if ((gasAccount != null || userConfig.ElectricityAccount()?.NewDuelFuelAccountConfigurator?.Prn != null) &&
			    !isClosingDualGas)
			{
				Assert.AreEqual("Gas", GasMeterReadingsHeader.TextContent);
				var gprn = (gasAccount?.NewPremise.GasPrn ??
				            userConfig.ElectricityAccount()?.NewDuelFuelAccountConfigurator?.Prn).ToString();
				Assert.IsTrue(GprnHeader.TextContent.Contains(gprn));
				Assert.IsTrue(GasMeterTypeLabel.TextContent.Contains("Gas Meter Reading *"));
				Assert.IsTrue(GasMeterReadingDescription.TextContent.Contains(
					"We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start."));
			}
			else
			{
				Assert.IsNull(GasMeterReadingsHeader);
				Assert.IsNull(GprnHeader);
			}
		}

		public void AssertInitialElectricityViewComponents(AppUserConfigurator userConfig)
		{
			var accountConfigurator = userConfig.ElectricityAccount();
			if (accountConfigurator != null || userConfig.GasAccount()?.NewDuelFuelAccountConfigurator?.Prn != null)
			{
				var register = (accountConfigurator != null
					? accountConfigurator.NewPremise.PremiseInfo.Installations.First().Devices.First()
					: userConfig.GasAccount().NewDuelFuelAccountConfigurator.Devices.Single()).Registers.First();

				Assert.AreEqual("Electricity", ElectricityMeterReadingsHeader.TextContent);
				var mprn = userConfig.ElectricityAndGasAccountConfigurators
					           .FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity)
					           ?.NewPremise.ElectricityPrn ??
				           userConfig.GasAccount().NewDuelFuelAccountConfigurator.Prn.ToString();

				Assert.IsTrue(MprnHeader.TextContent.Contains(mprn.ToString()));
				Assert.IsTrue(ElectricityMeterTypeLabel.TextContent.Contains(register.MeterType));

				Assert.IsTrue(ElectricityMeterReadingDescription.TextContent.Contains(
					"We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start."));
			}
			else
			{
				Assert.IsNull(ElectricityMeterReadingsHeader);
				Assert.IsNull(MprnHeader);
			}
		}
	}
}
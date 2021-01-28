using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step1InputMoveOutPage : MyAccountElectricityAndGasPage
	{
		public Step1InputMoveOutPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='mimo-1']") as IHtmlElement;

		public IHtmlInputElement MoveOutDatePicker =>
			Page.QuerySelector("#move_out") as IHtmlInputElement;

		public IHtmlLabelElement ElecMeterReadingTypeTitle =>
			Page.QuerySelector("#lblElecMeterType") as IHtmlLabelElement;

		public IHtmlLabelElement GasMeterReadingTypeTitle =>
			Page.QuerySelector("#lblGasMeterType") as IHtmlLabelElement;

		public IHtmlDivElement MPRNHeader =>
			Page.QuerySelector("#MprnHeader") as IHtmlDivElement;

		public IHtmlDivElement GPRNHeader =>
			Page.QuerySelector("#GprnHeader") as IHtmlDivElement;

		public IEnumerable<IHtmlElement> GeneralTermsAndConditionsLinks =>
			Page.QuerySelectorAll<IHtmlElement>("#termsAndConditions a");

		public IHtmlInputElement IncomingOccupantNoCheckedBox =>
			Page.QuerySelector("#incomingOccupantNo") as IHtmlInputElement;

		public IHtmlInputElement IncomingOccupantYesCheckedBox =>
			Page.QuerySelector("#incomingOccupantYes") as IHtmlInputElement;

		public IHtmlSpanElement LettingNameError =>
			Page.QuerySelector("#lettingAgentNameError") as IHtmlSpanElement;

		public IHtmlSpanElement LettingPhoneNumberError =>
			Page.QuerySelector("#lettingPhoneNumberError") as IHtmlSpanElement;

		public IHtmlSpanElement OccupierDetailsAcceptedError =>
			Page.QuerySelector("#occupierDetailsAcceptedError") as IHtmlSpanElement;

		public IHtmlSpanElement DayElecReadingError =>
			Page.QuerySelector("#dayElectricityMeterError") as IHtmlSpanElement;

		public IHtmlSpanElement NightElectricityMeterError =>
			Page.QuerySelector("#nightElectricityMeterError") as IHtmlSpanElement;

		public IHtmlSpanElement GasMeterReadingError =>
			Page.QuerySelector("#gasMeterReadingError") as IHtmlSpanElement;

		public IHtmlSpanElement OneElectricityMeterReadingError =>
			Page.QuerySelector("#oneElectricityMeterError") as IHtmlSpanElement;

		public IHtmlInputElement CheckBoxDetails =>
			Page.QuerySelector("#details") as IHtmlInputElement;

		public IHtmlInputElement CheckBoxTerms =>
			Page.QuerySelector("#terms") as IHtmlInputElement;

		public IHtmlElement HasReadGeneralTermsAndConditionsSection =>
			Page.QuerySelector("#termsAndConditions") as IHtmlElement;

		public IHtmlParagraphElement GasMeterReadingDescription =>
			Page.QuerySelector("#pGasMeterReadingDescription") as IHtmlParagraphElement;

		public IHtmlParagraphElement ElectricityMeterReadingDescription =>
			Page.QuerySelector("#pElectricityMeterReadingDescription") as IHtmlParagraphElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("1. Previous Property Details | Moving House"));
			}

			return isInPage;
		}

		public IHtmlInputElement GetGasReadingInput(string meterNumber)
		{
			return Page.QuerySelector("#txt_gas_0_" + meterNumber) as IHtmlInputElement;
		}

		public IHtmlInputElement GetElecReadingInput(string meterNumber)
		{
			return Page.QuerySelector("#txt_elec_" + meterNumber) as IHtmlInputElement;
		}

		public IHtmlButtonElement GetNextPRNButton()
		{
			return Page.QuerySelector("#btnNextPRN") as IHtmlButtonElement;
		}


		#region Helpers

		public Step1InputMoveOutPage InputFormValues(AppUserConfigurator configuration)
		{
			MoveOutDatePicker.Value = DateTime.Today.Subtract(TimeSpan.FromDays(3.0)).ToShortDateString();

			var accountConfigurators = configuration.ElectricityAndGasAccountConfigurators.ToArray();

			foreach (var accountConfigurator in accountConfigurators)
			{
				var inputElement = ResolveMeterInputElement(accountConfigurator);

				inputElement.Value =
					new Random((int) DateTime.UtcNow.Ticks).Next(1000, 2000).ToString();
			}

			IncomingOccupantNoCheckedBox.IsChecked = true;
			IncomingOccupantYesCheckedBox.IsChecked = false;
			CheckBoxDetails.IsChecked = true;
			CheckBoxTerms.IsChecked = true;
			return this;

			IHtmlInputElement ResolveMeterInputElement(CommonElectricityAndGasAccountConfigurator accountConfigurator)
			{
				if (accountConfigurators.Count(x =>
					x.Model.ClientAccountType == accountConfigurator.Model.ClientAccountType) != 1)
					throw new NotImplementedException("Only one account per type supported");
				IHtmlInputElement inputElement;
				if (accountConfigurator.Model.ClientAccountType == ClientAccountType.Electricity)
				{
					var registerInfo = accountConfigurator.Premise.ElectricityDevice().Registers.Single();

					if (registerInfo.MeterType != MeterType.Electricity24h)
						throw new NotImplementedException("Only 24h meter supported");

					inputElement = GetElecReadingInput(registerInfo.MeterNumber);
				}
				else if (accountConfigurator.Model.ClientAccountType == ClientAccountType.Gas)
				{
					var registerInfo = accountConfigurator.Premise.GasDevice().Registers.Single();
					inputElement = GetGasReadingInput(registerInfo.MeterNumber);
				}
				else
				{
					throw new NotImplementedException();
				}

				return inputElement;
			}
		}

		#endregion
	}
}
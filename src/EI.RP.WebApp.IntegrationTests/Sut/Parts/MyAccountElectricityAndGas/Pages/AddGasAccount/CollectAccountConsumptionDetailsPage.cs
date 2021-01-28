using System.Linq;
using AngleSharp.Html.Dom;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount
{
	internal class CollectAccountConsumptionDetailsPage : MyAccountElectricityAndGasPage
	{
		public CollectAccountConsumptionDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public virtual IHtmlElement Page =>
			Document.QuerySelector("[data-page='add-gas-consumption-details']") as IHtmlElement;

		public IHtmlButtonElement SubmitButton => Document
			.QuerySelector("#gasAccount") as IHtmlButtonElement;

		public IHtmlAnchorElement CancelButton => Document
			.QuerySelector("#btnCancel") as IHtmlAnchorElement;

		public IHtmlInputElement GPRN => Document
			.QuerySelector("#inputGPRN") as IHtmlInputElement;

		public IHtmlInputElement GasMeterReading => Document
			.QuerySelector("#GasMeterReading") as IHtmlInputElement;

		public IHtmlInputElement AuthorizationCheck =>
			(IHtmlInputElement) Document.QuerySelector("#chkAuthorization");

		public IHtmlInputElement TermsAndConditionsAccepted =>
			(IHtmlInputElement) Document.QuerySelector("#chkTermsAndConditionsAccepted");

		public IHtmlInputElement DebtFlagAndArrearsTermsAndConditions =>
			(IHtmlInputElement) Document.QuerySelector("#chkDebtFlagAndArrearsTermsAndConditions");


		public IHtmlInputElement PricePlanTermsAndConditions =>
			(IHtmlInputElement) Document.QuerySelector("#chkPricePlanTermsAndCondition");

		public IHtmlLabelElement GPRNLabel =>
			(IHtmlLabelElement) Document.QuerySelector("#lblGPRN");

		public IHtmlLabelElement MeterReadLabel =>
			(IHtmlLabelElement) Document.QuerySelector("#lblMeterRead");

		public virtual IHtmlParagraphElement ParagraphContent =>
			(IHtmlParagraphElement) Document.QuerySelector("[data-testid='add-gas-consumption-details-description']");

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Add Gas"));
			}

			return isInPage;
		}

		public CollectAccountConsumptionDetailsPage InputFormValues(AppUserConfigurator userConfig, int meterReading)
		{
			GPRN.Value = (string) userConfig.ElectricityAccounts().Single().NewDuelFuelAccountConfigurator.Prn;
			GasMeterReading.Value = meterReading.ToString();
			AuthorizationCheck.IsChecked = true;
			TermsAndConditionsAccepted.IsChecked = true;
			DebtFlagAndArrearsTermsAndConditions.IsChecked = true;
			PricePlanTermsAndConditions.IsChecked = true;
			return this;
		}
	}
}
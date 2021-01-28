using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class Step4BillingFrequencyPage : SmartActivationPage
	{
		public Step4BillingFrequencyPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("4. Billing Options | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='step4-billing-frequency']");
		public IHtmlSelectElement BillingDayOfMonthOption => (IHtmlSelectElement)Page.QuerySelector("#BillingDayOfMonth");
		public IHtmlInputElement BillingFrequencyTypeEveryTwoMonths => (IHtmlInputElement)Page.QuerySelector("#billingFrequencyTypeEveryTwoMonths");
		public IHtmlInputElement BillingFrequencyTypeEveryMonth => (IHtmlInputElement)Page.QuerySelector("#billingFrequencyTypeEveryMonth");

		public IHtmlElement AlternativePayerNote => (IHtmlElement)Page.QuerySelector("[data-testid='alternativePayerNote']");
		public IHtmlElement StandardPayerNote => (IHtmlElement)Page.QuerySelector("[data-testid='standardPayerNote']");
		public IHtmlElement ContinueButton => (IHtmlElement)Page.QuerySelector("#continueButton");

		public async Task<ISutApp> NextPage(BillingFrequencyType billingFrequencyType, int billingDayOfMonth = 1)
		{
			switch (billingFrequencyType)
			{
				case BillingFrequencyType.EveryMonth:
					BillingFrequencyTypeEveryMonth.IsChecked = true;
					BillingDayOfMonthOption.Value = billingDayOfMonth.ToString();
					break;
				case BillingFrequencyType.EveryTwoMonths:
					BillingFrequencyTypeEveryTwoMonths.IsChecked = true;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(billingFrequencyType));
			}
			return await ClickOnElement(ContinueButton);
		}
	}
}
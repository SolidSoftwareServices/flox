using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment
{
	internal class MakeAPaymentPage_ChangeAmount : MakeAPaymentPage
	{
		public MakeAPaymentPage_ChangeAmount(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlInputElement InputBox =>
			Document.QuerySelector("[data-testid='change-payment-amount-amount-input']") as IHtmlInputElement;

		public ChangePayAmount ChangeAmount => new ChangePayAmount(Document);

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='change-payment-amount']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Change Payment Amount"));
			}

			return isInPage;
		}

		public async Task<ResidentialPortalApp> ClickSubmit()
		{
			return (ResidentialPortalApp) await App.ClickOnElement(ChangeAmount.SubmitButton());
		}

		public async Task<ResidentialPortalApp> ClickCancel()
		{
			return (ResidentialPortalApp) await App.ClickOnElement(ChangeAmount.CancelButton());
		}

		public class ChangePayAmount
		{
			public ChangePayAmount(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public IHtmlButtonElement SubmitButton()
			{
				return Document.QuerySelector("[data-testid='change-payment-amount-submit']") as IHtmlButtonElement;
			}

			public IHtmlButtonElement CancelButton()
			{
				return Document.QuerySelector("[data-testid='change-payment-amount-cancel']") as IHtmlButtonElement;
			}
		}
	}
}
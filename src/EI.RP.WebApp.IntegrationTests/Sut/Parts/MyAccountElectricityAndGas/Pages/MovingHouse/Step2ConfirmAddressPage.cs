using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step2ConfirmAddressPage : MyAccountElectricityAndGasPage
	{
		public Step2ConfirmAddressPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='mimo-2-confirm']") as IHtmlElement;

		public IHtmlElement AddressTitle =>
			Document.QuerySelector("#addressTitle") as IHtmlElement;

		public IEnumerable<IHtmlDivElement> AddressContainers =>
			Document.QuerySelectorAll<IHtmlDivElement>("[data-testid='address-container']");

		public IHtmlParagraphElement MPRNTitleAndNumber =>
			AddressContainers?.ElementAtOrDefault(0)
				?.QuerySelector("[data-testid='address-container-title-and-number']") as IHtmlParagraphElement;

		public IHtmlParagraphElement MPRNAddress =>
			AddressContainers?.ElementAtOrDefault(0)?.QuerySelector("[data-testid='address-container-address']") as
				IHtmlParagraphElement;

		public IHtmlParagraphElement GPRNTitleAndNumberOnlyIfGasAccountExits =>
			AddressContainers?.ElementAtOrDefault(0)
				?.QuerySelector("[data-testid='address-container-title-and-number']") as IHtmlParagraphElement;

		public IHtmlParagraphElement GPRNAddressOnlyIfGasAccountExits =>
			AddressContainers?.ElementAtOrDefault(0)?.QuerySelector("[data-testid='address-container-address']") as
				IHtmlParagraphElement;

		public IHtmlParagraphElement GPRNTitleAndNumber =>
			AddressContainers?.ElementAtOrDefault(1)
				?.QuerySelector("[data-testid='address-container-title-and-number']") as IHtmlParagraphElement;

		public IHtmlParagraphElement GPRNAddress =>
			AddressContainers?.ElementAtOrDefault(1)?.QuerySelector("[data-testid='address-container-address']") as
				IHtmlParagraphElement;

		public IHtmlElement NeedHelp =>
			Document.QuerySelector("[data-testid='mimo-2-help-message']") as IHtmlElement;

		public IHtmlButtonElement ButtonContinue =>
			Document.QuerySelector("#btnContinue") as IHtmlButtonElement;

		public IHtmlButtonElement ButtonReEnter =>
			Document.QuerySelector("#new_mprn_gprn") as IHtmlButtonElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;
			
			return isInPage;
		}

		public async Task<Step2ConfirmAddressPage> ClickOnContinue()
		{
			await ClickOnElement(ButtonContinue);
			return this;
		}
	}
}
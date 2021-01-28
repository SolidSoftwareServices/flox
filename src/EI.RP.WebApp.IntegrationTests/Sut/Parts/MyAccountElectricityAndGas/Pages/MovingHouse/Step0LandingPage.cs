using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step0LandingPage : MovingHomePage
	{
		public Step0LandingPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page => Document
			.QuerySelector("[data-page='mimo-0']") as IHtmlElement;

		public IHtmlHeadingElement OnMovingDay => Page?
			.QuerySelector("[data-testid='mimo-0-on-moving-day-title']") as IHtmlHeadingElement;

		public IHtmlHeadingElement AfterYouMoveIn => Page?
			.QuerySelector("[data-testid='mimo-0-after-you-move-in-title']") as IHtmlHeadingElement;

		public IHtmlHeadingElement CompleteMoveOnline => Page?
			.QuerySelector("[data-testid='mimo-0-complete-move-online-title']") as IHtmlHeadingElement;

		public IHtmlParagraphElement AddressHeader => Page?
			.QuerySelector("[data-testid='mimo-0-address-title']") as IHtmlParagraphElement;

		public IHtmlParagraphElement AddressContent => Page?
			.QuerySelector("[data-testid='mimo-0-address-description']") as IHtmlParagraphElement;

		public IHtmlParagraphElement AccountName => Page?
			.QuerySelector("[data-testid='mimo-0-accounts']") as IHtmlParagraphElement;

		public IHtmlParagraphElement AccountNumberDetail => Page?
			.QuerySelector("[data-testid='mimo-0-accounts']") as IHtmlParagraphElement;

		public IHtmlAnchorElement MyAccountsList => Document
			.QuerySelector("[data-testid='main-navigation-my-accounts-link']") as IHtmlAnchorElement;

		public IHtmlElement ExitFeeCloseBothDetail => Document
			.QuerySelector("#exitFeeCloseBothDetail") as IHtmlElement;

		public IHtmlElement ExitFeeMoveAndCloseDetail => Document
			.QuerySelector("#exitFeeMoveAndCloseDetail") as IHtmlElement;

		public IHtmlElement InstalmentPlanDetail => Document
			.QuerySelector("#instalmentPlanDetail") as IHtmlElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Moving House"));
			}

			return isInPage;
		}

		public async Task ClickOnPopUpButton1()
		{
			await ClickOnElement(PopupButton1);
		}

		public async Task ClickOnPopUpButton2()
		{
			await ClickOnElement(PopupButton2);
		}

		public async Task ClickOnPopUpButton3()
		{
			await ClickOnElement(PopupButton3);
		}

		#region electricity

		public IHtmlAnchorElement MoveElectricity => Page?
			.QuerySelector("#btnMoveElec") as IHtmlAnchorElement;

		public IHtmlAnchorElement MoveElectricityAndAddGas => Page?
			.QuerySelector("#btnMoveElecAndAddGas") as IHtmlAnchorElement;

		public IHtmlAnchorElement CloseElectricity => Page?
			.QuerySelector("#btnCloseElec") as IHtmlAnchorElement;

		public IHtmlButtonElement PopupButton1 => Document
			.QuerySelector("#step0-pu-button1") as IHtmlButtonElement;

		public IHtmlButtonElement PopupButton2 => Document
			.QuerySelector("#step0-pu-button2") as IHtmlButtonElement;

		public IHtmlButtonElement PopupButton3 => Document
			.QuerySelector("#step0-pu-button3") as IHtmlButtonElement;

		#endregion

		#region gas

		public IHtmlAnchorElement MoveGas => Page?
			.QuerySelector("#btnMoveGas") as IHtmlAnchorElement;

		public IHtmlAnchorElement MoveGasAndAddElectricity => Page?
			.QuerySelector("#btnMoveGasAndAddElec") as IHtmlAnchorElement;

		public IHtmlAnchorElement CloseGas => Page?
			.QuerySelector("#btnCloseGas") as IHtmlAnchorElement;

		#endregion

		#region duelfuel

		public IHtmlAnchorElement MoveGasAndElec => Page?
			.QuerySelector("#btnMoveElecAndGas") as IHtmlAnchorElement;

		public IHtmlAnchorElement MoveElectricityAndCloseGas => Page?
			.QuerySelector("#btnMoveElecAndCloseGas") as IHtmlAnchorElement;

		public IHtmlAnchorElement CloseGasAndEle => Page?
			.QuerySelector("#btnCloseElecAndGas") as IHtmlAnchorElement;

		#endregion
	}
}
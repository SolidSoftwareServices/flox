using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step2InputPrnsPage : MyAccountElectricityAndGasPage
	{
		public Step2InputPrnsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='mimo-2-input']") as IHtmlElement;

		public IHtmlHeadingElement InputTitle => Document.QuerySelector("#inputTitle") as IHtmlHeadingElement;
		public IHtmlInputElement GPRNInput => Document.QuerySelector("#gprn") as IHtmlInputElement;

		public IHtmlElement GPRNInputError =>
			Document.QuerySelector("#inputGPRN span[data-valmsg-for='NewGPRN']") as IHtmlElement;

		public IHtmlElement GasOnlyGPRNInputError =>
			Document.QuerySelector("#inputMPRN span[data-valmsg-for='NewGPRN']") as IHtmlElement;

		public IHtmlInputElement MPRNInput => Document.QuerySelector("#mprn") as IHtmlInputElement;

		public IHtmlElement MPRNInputError =>
			Document.QuerySelector("#inputMPRN span[data-valmsg-for='NewMPRN']") as IHtmlElement;


		public IHtmlLabelElement GPRNLabel => Document.QuerySelector("#lblGPRN") as IHtmlLabelElement;
		public IHtmlLabelElement MPRNLabel => Document.QuerySelector("#lblMRPN") as IHtmlLabelElement;

		public IHtmlButtonElement SubmitPRNS => Document.QuerySelector("#submitNewPRNs") as IHtmlButtonElement;

		public IHtmlAnchorElement CancelButton => Document.QuerySelector("#btnCancel") as IHtmlAnchorElement;

		public IHtmlAnchorElement CancelMovePage => Document.QuerySelector("#cancelMoveHouse") as IHtmlAnchorElement;

		public IHtmlElement NeedHelp => Document.QuerySelector("#needHelpContent") as IHtmlElement;

		public IHtmlButtonElement TrySubmitNewPRNS => Document.QuerySelector("#btnTryNewPRN") as IHtmlButtonElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;
			
			return isInPage;
		}

		public Step2InputPrnsPage InputFormValues(AppUserConfigurator userConfig)
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			SetElectricity();

			SetGas();

			return this;

			void SetGas()
			{
				if (GPRNInput != null)
				{
					var gasAccount =
						userConfig.ElectricityAndGasAccountConfigurators.SingleOrDefault(x =>
							x.Model.ClientAccountType == ClientAccountType.Gas);
					if (gasAccount != null)
					{
						GPRNInput.Value = (string) gasAccount.NewPremise.GasPrn;
					}
					else
					{
						var electricity = userConfig.ElectricityAccount();
						GPRNInput.Value = (string) electricity.NewDuelFuelAccountConfigurator.Prn;
					}
				}
			}

			void SetElectricity()
			{
				if (MPRNInput != null)
				{
					var electricityAccount =
						userConfig.ElectricityAndGasAccountConfigurators.SingleOrDefault(x =>
							x.Model.ClientAccountType == ClientAccountType.Electricity);
					if (electricityAccount != null)
						MPRNInput.Value = (string) electricityAccount.NewPremise.ElectricityPrn;
					else
						MPRNInput.Value =
							(string) userConfig.GasAccount().NewDuelFuelAccountConfigurator.Prn;
				}
			}
		}

		public Step2InputPrnsPage InputFormValues_NewProperty(AppUserConfigurator userConfig)
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			SetElectricity();

			SetGas();

			return this;

			void SetGas()
			{
				if (GPRNInput != null)
				{
					var gasAccount =
						userConfig.ElectricityAndGasAccountConfigurators.SingleOrDefault(x =>
							x.Model.ClientAccountType == ClientAccountType.Gas);
					if (gasAccount != null)
						GPRNInput.Value = (string) gasAccount.NewPremise.GasPrn;
					else
						GPRNInput.Value = (string) fixture.Create<GasPointReferenceNumber>();
				}
			}

			void SetElectricity()
			{
				var electricityAccount =
					userConfig.ElectricityAndGasAccountConfigurators.SingleOrDefault(x =>
						x.Model.ClientAccountType == ClientAccountType.Electricity);
				if (MPRNInput != null)
				{
					if (electricityAccount != null)
						MPRNInput.Value = (string) electricityAccount.NewPremise.ElectricityPrn;
					else
						MPRNInput.Value =
							(string) userConfig.ElectricityAndGasAccountConfigurators.SingleOrDefault()
								?.NewDuelFuelAccountConfigurator.Prn;
				}
			}
		}

		public async Task<Step2InputPrnsPage> ClickOnSubmitPrns()
		{
			await ClickOnElement(SubmitPRNS);
			return this;
		}

		internal static class ValidationMessages
		{
			public static string MprnIsTheSameAsTheHomeYouAreLeaving =
				"The MPRN you entered is the same as the MPRN for the home you are leaving. Please enter new MPRN.";

			public static string GprnIsTheSameAsTheHomeYouAreLeaving =
				"The GPRN you entered is the same as the GPRN for the home you are leaving. Please enter new GPRN.";
		}
	}
}
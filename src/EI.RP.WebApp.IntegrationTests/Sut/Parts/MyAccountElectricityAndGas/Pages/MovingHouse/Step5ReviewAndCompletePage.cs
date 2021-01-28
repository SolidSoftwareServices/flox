using System;
using System.Linq;
using AngleSharp.Html.Dom;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step5ReviewAndCompletePage : MyAccountElectricityAndGasPage
	{
		public Step5ReviewAndCompletePage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='mimo-5-review-and-complete']") as IHtmlElement;

		public IHtmlHeadingElement ReviewDetailsHeader =>
			Document.QuerySelector("#reviewDetailsHeader") as IHtmlHeadingElement;

		public IHtmlElement ReviewDetailsContent => Document.QuerySelector("#reviewDetailsContent") as IHtmlElement;

		public IHtmlElement CompleteMoveHouse => Document.QuerySelector("#btnCompleteMoveHouse") as IHtmlElement;

		public AccountDetails ShowAccountDetails => new AccountDetails(Document);

		public PropertyDetails ShowPropertyDetails => new PropertyDetails(Document);

		public MovingDatesAndMeterReadings ShowMovingDatesAndMeterReadings => new MovingDatesAndMeterReadings(Document);

		public Payments ShowPayments => new Payments(Document);

		public PricePlan ShowPricePlan => new PricePlan(Document);

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("5. Review | Moving House"));
			}

			return isInPage;
		}

		public class AccountDetails
		{
			public AccountDetails(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }


			public IHtmlHeadingElement AccountDetailsHeader =>
				Document.QuerySelector("#accountDetailsHeader") as IHtmlHeadingElement;

			public IHtmlElement AccountDetailsPrimaryAccountType =>
				Document.QuerySelector("#primaryAccountType") as IHtmlElement;

			public IHtmlElement AccountDetailsPrimaryAccountNumber =>
				Document.QuerySelector("#primaryAccountNumber") as IHtmlElement;

			public IHtmlElement AccountDetailsSecondaryAccountType =>
				Document.QuerySelector("#secondaryAccountType") as IHtmlElement;

			public IHtmlElement AccountDetailsSecondaryAccountNumber =>
				Document.QuerySelector("#secondaryAccountNumber") as IHtmlElement;

			public IHtmlElement AccountDetailsElectricityAccountType =>
				Document.QuerySelector("#electricityAccountType") as IHtmlElement;

			public IHtmlElement AccountDetailsElectricityAccountNumber =>
				Document.QuerySelector("#electricityAccountNumber") as IHtmlElement;

			public IHtmlElement AccountDetailsGasAccountType =>
				Document.QuerySelector("#gasAccountType") as IHtmlElement;

			public IHtmlElement AccountDetailsGasAccountNumber =>
				Document.QuerySelector("#gasAccountNumber") as IHtmlElement;

			public IHtmlElement NewElectricityAccountText =>
				Document.QuerySelector("#newElectricityAccountText") as IHtmlElement;

			public IHtmlElement NewGasAccountText =>
				Document.QuerySelector("#newGasAccountText") as IHtmlElement;

			public IHtmlAnchorElement StartOverButton =>
				Document.QuerySelector("#btnStartOver") as IHtmlAnchorElement;
			public IHtmlElement GetElecReadingMoveOutElement(string meterNumber)
			{
				var readingElementId = $"[data-testid='id_elec_moveout_reading_{meterNumber.TrimStart('0')}']";
				return Document.QuerySelector(readingElementId) as IHtmlElement;
			}

			public IHtmlElement GetElecReadingMoveOutResult(string meterNumber)
			{
				var readingResultId = $"[data-testid='id_elec_moveout_reading_value_{meterNumber.TrimStart('0')}']";
				return Document.QuerySelector(readingResultId) as IHtmlElement;
			}

			public IHtmlElement GetElecReadingMoveInElement(string meterNumber)
			{
				var readingElementId = $"[data-testid='id_elec_movein_reading_{meterNumber.TrimStart('0')}']";
				return Document.QuerySelector(readingElementId) as IHtmlElement;
			}

			public IHtmlElement GetElecReadingMoveInResult(string meterNumber)
			{
				var readingResultId = $"[data-testid='id_elec_movein_reading_value_{meterNumber.TrimStart('0')}']";
				return Document.QuerySelector(readingResultId) as IHtmlElement;
			}
		}

		public class PropertyDetails
		{
			public PropertyDetails(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }


			public IHtmlElement PreviousPropertyHeader =>
				Document.QuerySelector("#previousProperty") as IHtmlElement;

			public IHtmlElement PreviousAddressInfo =>
				Document.QuerySelector("#previousAddressInfo") as IHtmlElement;

			public IHtmlElement NewPropertyHeader =>
				Document.QuerySelector("#newProperty") as IHtmlElement;

			public IHtmlElement NewAddressInfo =>
				Document.QuerySelector("#newAddressInfo") as IHtmlElement;

			public IHtmlButtonElement EditNewAddressButton =>
				Document.QuerySelector("#btnEditNewAddress") as IHtmlButtonElement;
		}

		public class MovingDatesAndMeterReadings
		{
			public MovingDatesAndMeterReadings(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public IHtmlHeadingElement MovingDateHeader =>
				Document.QuerySelector("#movingDateHeader") as IHtmlHeadingElement;

			public IHtmlElement MoveOutDateTitle =>
				Document.QuerySelector("#moveOutDateTitle") as IHtmlElement;

			public IHtmlElement MoveOutDate =>
				Document.QuerySelector("#moveOutDate") as IHtmlElement;

			public IHtmlElement MoveOutElectricityClientType =>
				Document.QuerySelector("#moveOutElectricityClientType") as IHtmlElement;

			public IHtmlElement MoveOutElectricityMeterReading =>
				Document.QuerySelector("#moveOutElectricityMeterReading") as IHtmlElement;

			public IHtmlElement MoveOutElectricityMeterReadingValue =>
				Document.QuerySelector("#moveOutElectricityMeterReadingValue") as IHtmlElement;

			public IHtmlElement MoveInElectricityMeterReadingValue =>
				Document.QuerySelector("#moveInElectricityMeterReadingValue") as IHtmlElement;

			public IHtmlElement MoveOutGasClientType =>
				Document.QuerySelector("#moveOutGasClientType") as IHtmlElement;

			public IHtmlElement MoveOutGasMeterReading =>
				Document.QuerySelector("#moveOutGasMeterReading") as IHtmlElement;

			public IHtmlElement MoveOutGasMeterReadingValue =>
				Document.QuerySelector("#moveOutGasMeterReadingValue") as IHtmlElement;

			public IHtmlElement MoveInGasMeterReadingValue =>
				Document.QuerySelector("#moveInGasMeterReadingValue") as IHtmlElement;

			public IHtmlButtonElement EditMoveOutDetailsButton =>
				Document.QuerySelector("#btnEditMoveOutDetails") as IHtmlButtonElement;

			public IHtmlButtonElement EditMoveInDetailsButton =>
				Document.QuerySelector("#btnEditMoveInDetails") as IHtmlButtonElement;
		}

		public class Payments
		{
			public Payments(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public IHtmlHeadingElement PaymentHeader =>
				Document.QuerySelector("#paymentMethodHeader") as IHtmlHeadingElement;

			public IHtmlElement PrimaryAccountType =>
				Document.QuerySelector("#primaryAccountType") as IHtmlElement;

			public IHtmlElement PrimaryPaymentType =>
				Document.QuerySelector("#primaryPaymentType") as IHtmlElement;


			public IHtmlButtonElement PrimaryPaymentEdit =>
				Document.QuerySelector("#primaryPaymentEdit") as IHtmlButtonElement;


			public IHtmlElement SecondaryAccountType =>
				Document.QuerySelector("#secondaryAccountType") as IHtmlElement;

			public IHtmlElement SecondaryPaymentType =>
				Document.QuerySelector("#secondaryPaymentType") as IHtmlElement;


			public IHtmlButtonElement SecondaryPaymentEdit =>
				Document.QuerySelector("#secondaryPaymentEdit") as IHtmlButtonElement;
		}

		public class PricePlan
		{
			public PricePlan(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public IHtmlHeadingElement PricePlanHeader =>
				Document.QuerySelector("#pricePlanHeader") as IHtmlHeadingElement;

			public IHtmlElement PricePlanText =>
				Document.QuerySelector("#pricePlan") as IHtmlElement;
		}

		public class PaymentPart
		{
			public PaymentPart(IHtmlDivElement src)
			{
				var lines = src.TextContent.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x))
					.ToArray();
				try
				{
					PaymentType = lines[0].Split(':')[1].ToEnum<PaymentSetUpType>(false);
					Iban = lines[1].Split(':')[1];
					AccountName = lines[2].Split(':')[1];
					PaymentAccountNumber = lines[3];
					PaymentAccountType = (ClientAccountType) lines[4];
				}
				catch (Exception ex)
				{
				}
			}

			public ClientAccountType PaymentAccountType { get; }

			public string PaymentAccountNumber { get; }

			public string AccountName { get; }

			public string Iban { get; }

			public PaymentSetUpType PaymentType { get; }
		}
	}
}
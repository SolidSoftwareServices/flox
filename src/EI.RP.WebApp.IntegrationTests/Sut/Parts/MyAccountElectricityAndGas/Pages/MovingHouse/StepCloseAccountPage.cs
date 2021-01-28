using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class StepCloseAccountPage : MyAccountElectricityAndGasPage
	{
		public StepCloseAccountPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='mimo-close-accounts']") as IHtmlElement;

		public IHtmlInputElement MoveOutDatePicker =>
			Page.QuerySelector("#move_out") as IHtmlInputElement;

		public IHtmlLabelElement DatePickerTitle =>
			Page.QuerySelector("#datePickerTitle") as IHtmlLabelElement;

		public IHtmlLabelElement ElecMeterReadingTypeTitle =>
			Page.QuerySelector("#lblElecMeterType") as IHtmlLabelElement;

		public IHtmlLabelElement GasMeterReadingTypeTitle =>
			Page.QuerySelector("#lblGasMeterType") as IHtmlLabelElement;

		public IHtmlDivElement MPRNHeader =>
			Page.QuerySelector("#MprnHeader") as IHtmlDivElement;

		public IHtmlDivElement GPRNHeader =>
			Page.QuerySelector("#GprnHeader") as IHtmlDivElement;

		public IHtmlInputElement IncomingOccupantNoCheckedBox =>
			Page.QuerySelector("#incomingOccupantNo") as IHtmlInputElement;

		public IHtmlInputElement IncomingOccupantYesCheckedBox =>
			Page.QuerySelector("#incomingOccupantYes") as IHtmlInputElement;

		public IHtmlInputElement IsPOBoxFieldRequired =>
			Page.QuerySelector("#isPOBoxFieldRequired") as IHtmlInputElement;

		public IHtmlInputElement IsNIFieldRequired =>
			Page.QuerySelector("#isNIFieldRequired") as IHtmlInputElement;

		public IHtmlInputElement IsROIFieldRequired =>
			Page.QuerySelector("#isROIBoxFieldRequired") as IHtmlInputElement;

		public IHtmlSpanElement LettingNameError =>
			Page.QuerySelector("#lettingAgentNameError") as IHtmlSpanElement;

		public IHtmlSpanElement LettingPhoneNumberError =>
			Page.QuerySelector("#lettingPhoneNumberError") as IHtmlSpanElement;

		public IHtmlSpanElement OccupierDetailsAcceptedError =>
			Page.QuerySelector("#occupierDetailsAcceptedError") as IHtmlSpanElement;

		public IHtmlSpanElement GasMeterReadingError =>
			Page.QuerySelector("#gasMeterReadingError") as IHtmlSpanElement;

		public IHtmlSpanElement OneElectricityMeterReadingError =>
			Page.QuerySelector("#oneElectricityMeterError") as IHtmlSpanElement;

		public IHtmlSpanElement DayElectricityMeterError =>
			Page.QuerySelector("#dayElectricityMeterError") as IHtmlSpanElement;

		public IHtmlSpanElement NightElectricityMeterError =>
			Page.QuerySelector("#nightElectricityMeterError") as IHtmlSpanElement;

		public IHtmlInputElement CheckBoxDetails =>
			Page.QuerySelector("#details") as IHtmlInputElement;

		public IHtmlInputElement CheckBoxTerms =>
			Page.QuerySelector("#terms") as IHtmlInputElement;

		public IHtmlParagraphElement GasMeterReadingDescription =>
			Page.QuerySelector("#pGasMeterReadingDescription") as IHtmlParagraphElement;

		public IHtmlParagraphElement ElectricityMeterReadingDescription =>
			Page.QuerySelector("#pElectricityMeterReadingDescription") as IHtmlParagraphElement;

		public IHtmlHeadingElement ForwardingAddressHeader =>
			Page.QuerySelector("#forwardingAddress") as IHtmlHeadingElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Close Account"));
			}

			return isInPage;
		}

		public IHtmlInputElement GetGasReadingInput(string meterNumber)
		{
			return Page.QuerySelector("#txt_gas_0_" + meterNumber) as IHtmlInputElement;
		}

		public IHtmlButtonElement GetCloseButton()
		{
			return Document.QuerySelector("#btnCloseAccount") as IHtmlButtonElement;
		}

		public IHtmlInputElement GetElectricityReadingInput(string meterNumber)
		{
			return Page.QuerySelector($"#txt_elec_{meterNumber}") as IHtmlInputElement;
		}

		#region [ROI]

		public IHtmlInputElement GetRoiAddressLine1InputElement()
		{
			return Document.QuerySelector("#roiAddressLine1") as IHtmlInputElement;
		}

		public IHtmlLabelElement GetRoiAddressLine2Element()
		{
			return Document.QuerySelector("#lblRoiAddressLine2") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetRoiAddressLine2InputElement()
		{
			return Document.QuerySelector("#roiAddressLine2") as IHtmlInputElement;
		}

		public IHtmlLabelElement GetRoiTownElement()
		{
			return Document.QuerySelector("#lblRoiTown") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetRoiTownInputElement()
		{
			return Document.QuerySelector("#roiTown") as IHtmlInputElement;
		}

		public IHtmlLabelElement GetRoiRoiCountyElement()
		{
			return Document.QuerySelector("#lblRoiCounty") as IHtmlLabelElement;
		}

		public IHtmlSelectElement GetRoiRoiCountyDropDownElement()
		{
			return Document.QuerySelector("#ROIAddress_County") as IHtmlSelectElement;
		}

		public IHtmlOptionElement GetRoiRoiCountyDropDownElementOption()
		{
			return Document.QuerySelector("#ROIAddress_County > option:nth-child(1)") as IHtmlOptionElement;
		}

		public IHtmlLabelElement GetRoiPostCodeElement()
		{
			return Document.QuerySelector("#lblRoiPostCode") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetRoiPostCodeInputElement()
		{
			return Document.QuerySelector("#roiPostalCode") as IHtmlInputElement;
		}

		public IHtmlSpanElement RoiHouseNumberError =>
			(IHtmlSpanElement) Document.QuerySelector("#roiHouseNumberError");

		public IHtmlSpanElement RoiStreetError =>
			Document.QuerySelector("#roiStreetError") as IHtmlSpanElement;

		public IHtmlSpanElement RoiTownError =>
			Document.QuerySelector("#roiTownError") as IHtmlSpanElement;

		public IHtmlSpanElement RoiCountyError =>
			Document.QuerySelector("#roiCountyError") as IHtmlSpanElement;

		public IHtmlLabelElement GetRoiHouseNumberElement()
		{
			return Document.QuerySelector("#lblRoiHouseNumber") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetRoiHouseNumberInputElement()
		{
			return Document.QuerySelector("#roiHouseNumber") as IHtmlInputElement;
		}

		public IHtmlLabelElement GetRoiStreetLabelElement()
		{
			return Document.QuerySelector("#lblRoiStreet") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetRoiStreetInputElement()
		{
			return Document.QuerySelector("#roiStreet") as IHtmlInputElement;
		}

		public IHtmlSpanElement GetRoiAddressHeadingElement()
		{
			return Document.QuerySelector("#ireTitle") as IHtmlSpanElement;
		}

		public IHtmlLabelElement GetRoiAddressLine1Element()
		{
			return Document.QuerySelector("#lblRoiAddressLine1") as IHtmlLabelElement;
		}

		#endregion

		#region [Po]

		public IHtmlLabelElement GetLabelPOBoxNumber()
		{
			return Document.QuerySelector("#lblPOBoxNumber") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelPOBoxPostCode()
		{
			return Document.QuerySelector("#lblPOBoxPostCode") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelPoDistrict()
		{
			return Document.QuerySelector("#lblPoDistrict") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelPoBoxCountry()
		{
			return Document.QuerySelector("#lblPoBoxCountry") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetPOBoxNumberTextBox()
		{
			return Document.QuerySelector("#POBoxNumber") as IHtmlInputElement;
		}

		public IHtmlInputElement GetPOBoxPostCodeTextBox()
		{
			return Document.QuerySelector("#POBoxPostCode") as IHtmlInputElement;
		}

		public IHtmlInputElement GetPoDistrictTextBox()
		{
			return Document.QuerySelector("#poDistrict") as IHtmlInputElement;
		}

		public IHtmlSpanElement PoPOBoxNumberError =>
			Document.QuerySelector("#poPOBoxNumberError") as IHtmlSpanElement;

		public IHtmlSpanElement PoPOBoxPostCodeError =>
			Document.QuerySelector("#poPOBoxPostCodeError") as IHtmlSpanElement;

		public IHtmlSpanElement PoCountryError =>
			Document.QuerySelector("#poCountryError") as IHtmlSpanElement;

		public IHtmlSpanElement PoDistrictError =>
			Document.QuerySelector("#poDistrictError") as IHtmlSpanElement;

		public IHtmlSelectElement GetPoCountry()
		{
			return Document.QuerySelector("#PostalAddress_Country") as IHtmlSelectElement;
		}

		public IHtmlOptionElement PostalAddressCountryOption()
		{
			return Document.QuerySelector("#PostalAddress_Country > option:nth-child(1)") as IHtmlOptionElement;
		}

		#endregion

		#region [NI]

		public IHtmlSpanElement GetNiHeader()
		{
			return Document.QuerySelector("#niHeader") as IHtmlSpanElement;
		}

		public IHtmlLabelElement GetLabelNICountryList()
		{
			return Document.QuerySelector("#lblNICountryList") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiAddressline1()
		{
			return Document.QuerySelector("#lblNiAddressline1") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiHouseNumber()
		{
			return Document.QuerySelector("#lblNiHouseNumber") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiStreet()
		{
			return Document.QuerySelector("#lblNiStreet") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiAddressline2()
		{
			return Document.QuerySelector("#lblNiAddressline2") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiTown()
		{
			return Document.QuerySelector("#lblNiTown") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiPostCode()
		{
			return Document.QuerySelector("#lblNiPostCode") as IHtmlLabelElement;
		}

		public IHtmlLabelElement GetLabelNiDistrict()
		{
			return Document.QuerySelector("#lblniDistrict") as IHtmlLabelElement;
		}

		public IHtmlInputElement GetniAddressline1()
		{
			return Document.QuerySelector("#niAddressline1") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiHouseNumber()
		{
			return Document.QuerySelector("#niHouseNumber") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiStreet()
		{
			return Document.QuerySelector("#niStreet") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiAddressline2()
		{
			return Document.QuerySelector("#niAddressline2") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiTown()
		{
			return Document.QuerySelector("#niTown") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiPostCode()
		{
			return Document.QuerySelector("#niPostCode") as IHtmlInputElement;
		}

		public IHtmlInputElement GetNiDistrict()
		{
			return Document.QuerySelector("#niDistrict") as IHtmlInputElement;
		}

		public IHtmlSelectElement GetNiCountry()
		{
			return Document.QuerySelector("#NIAddress_Country") as IHtmlSelectElement;
		}

		public IHtmlOptionElement GetNiCountryOption()
		{
			return Document.QuerySelector("#NIAddress_Country > option:nth-child(1)") as IHtmlOptionElement;
		}

		public IHtmlSpanElement NiCountryError =>
			Document.QuerySelector("#niCountryError") as IHtmlSpanElement;

		public IHtmlSpanElement NiHouseNumberError =>
			Document.QuerySelector("#niHouseNumberError") as IHtmlSpanElement;

		public IHtmlSpanElement NiStreetError =>
			Document.QuerySelector("#niStreetError") as IHtmlSpanElement;

		public IHtmlSpanElement NiTownError =>
			Document.QuerySelector("#niTownError") as IHtmlSpanElement;

		public IHtmlSpanElement NiPostCodeError =>
			Document.QuerySelector("#niPostCodeError") as IHtmlSpanElement;

		public IHtmlSpanElement NiDistrictError =>
			Document.QuerySelector("#niDistrictError") as IHtmlSpanElement;

		public IEnumerable<IHtmlElement> GeneralTermsAndConditionsLinks =>
			Document.QuerySelectorAll<IHtmlElement>("#termsAndConditions a");

		#endregion
	}
}
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseStepFive
    {
        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
        internal MoveHouseStepFive(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseStepFive(IWebDriver driver0)
        {
            driver = driver0;
        }

        WebDriverWait wait;

        internal string
            step5HeaderID = "step5Header",
            reviewDetailsHeaderID = "reviewDetailsHeader",
            reviewDetailsSubTextID = "reviewDetailsContent",
            accountDetailsHeaderID = "accountDetailsHeader",
            elecHeaderID = "electricityAccountType",
            elecTextID = "electricityAccountNumber",
            gasHeaderID = "gasAccountType",
            gasTextID = "newGasAccountText",
            propertyDetailsHeaderID = "propertyDetailsHeader",
            propertyDetailsPreviousHeaderID = "previousProperty",
            propertyDetailsPreviousTextID = "previousAddressInfo",
            propertyDetailsNewHeaderID = "newProperty",
            propertyDetailsNewTextID = "newAddressInfo",
            editNewPropertyDetailsBtnID = "btnEditNewAddress",
            moveDateMeterHeaderID = "movingDateHeader",
            moveDatePreviousPropertyHeaderID = "previosuPropertyMeterReadings",
            moveDateMoveOutDateTextID = "moveOutDateTitle",
            moveDateMoveOutDateID = "moveOutDate",
            moveDateNewPropertyHeaderID = "newPropertyMoveInDates",
            moveDateMoveInDateTextID = "moveInDateTitle",
            moveDateMoveInDateID = "moveInDate",
            moveDateMoveOutElecHeaderID = "moveOutElectricityClientType",
            moveDateMoveOutElecMeterReadTextID = "moveOutElectricityMeterReading",
            moveDateMoveOutElecMeterReadID = "moveOutElectricityMeterReadingValue",
            moveDateMoveInElecHeaderID = "moveInElectricityClientAccountType",
            moveDateMoveInElecMeterReadTextID = "moveInElectricityMeterReading",
            moveDateMoveInElecMeterReadID = "moveInElectricityMeterReadingValue",
            moveDateMoveInGasHeaderID = "moveInGas",
            moveDateMoveInGasMeterReadTextID = "moveInGasMeterReading",
            moveDateMoveInGasMeterReadID = "moveInGasMeterReadingValue",
            editMoveOutDetailsBtnID = "btnEditMoveOutDetails",
            editMoveInDetailsBtnID = "btnEditMoveInDetails",
            paymentMethodHeaderID = "paymentMethodHeader",
            paymentMethodElecHeaderID = "primaryAccountType",
            paymentMethodElecTypeID = "primaryPaymentType",
            paymentMethodGasHeaderID = "secondaryAccountType",
            paymentMethodGasTypeID = "secondaryPaymentType",
            editElecPaymentBtnID = "primaryPaymentEdit",
            editGasPaymentBtnID = "secondaryPaymentEdit",
            pricePlanHeaderID = "pricePlanHeader",
            pricePlanTextID = "pricePlan",
            completeHouseMoveBtnID = "btnCompleteMoveHouse";

        internal void ClickMoveHouseStepFivePropertyDetailsNewEditBtn()
        {
            _sharedPageFunctions.ClickElement(By.Id(editNewPropertyDetailsBtnID));
        }

        internal void AssertMoveHouseStepFivePage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStep5ActiveTab(sharedVariables);
            AssertMoveHouseStepFiveHeader();
            AssertMoveHouseStepFiveSubHeader();
            AssertMoveHouseStepFiveHeaderSubText();
            AssertMoveHouseStepFiveCompleteHouseMoveTopBtn();
            AssertMoveHouseStepFiveAccountDetailsHeader();
            AssertMoveHouseStepFiveAccountDetailsElecHeader();
            AssertMoveHouseStepFiveAccountDetailsElecText();
            AssertMoveHouseStepFiveAccountDetailsGasHeader();
            AssertMoveHouseStepFiveAccountDetailsGasText();
            AssertMoveHouseStepFivePropertyDetailsHeader();
            AssertMoveHouseStepFivePropertyDetailsPreviousHeader();
            AssertMoveHouseStepFivePropertyDetailsPreviousText();
            AssertMoveHouseStepFivePropertyDetailsNewHeader();
            AssertMoveHouseStepFivePropertyDetailsNewText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMoveOutDateText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMoveOutDate();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyMoveInDateText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyMoveInDate();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyElecHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyElecMeterReadText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMeterRead();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecMeterReadText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecMeterRead();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasHeader();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasMeterReadText();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasMeterRead();
            AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyEditBtn();
            AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyEditBtn();
            AssertMoveHouseStepFivePaymentMethodHeader();
            AssertMoveHouseStepFivePaymentMethodElecHeader();
            AssertMoveHouseStepFivePaymentMethodElecText();
            AssertMoveHouseStepFivePaymentMethodGasHeader();
            AssertMoveHouseStepFivePaymentMethodGasText();
            AssertMoveHouseStepFivePaymentMethodElecEditBtn();
            AssertMoveHouseStepFivePaymentMethodGasEditBtn();
            AssertMoveHouseStepFivePricePlanHeader();
            AssertMoveHouseStepFivePricePlanText();
            AssertMoveHouseStepFivePricePlanLink();
            AssertMoveHouseStepFiveCompleteHouseMoveBottomBtn();
            AssertMoveHouseStepFiveCancelBtn();
        }

        internal void AssertMoveHouseStep5ActiveTab(SharedVariables sharedVariables)
        {
	        _sharedPageFunctions.IsElementPresent(By.CssSelector("#step5.active"));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step1TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step2TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step3TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step4TabID));
        }

        internal void AssertMoveHouseStepFiveHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(step5HeaderID));
        }

        internal void AssertMoveHouseStepFiveSubHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(reviewDetailsHeaderID));
        }

        internal void AssertMoveHouseStepFiveHeaderSubText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(reviewDetailsSubTextID));
        }

        internal void AssertMoveHouseStepFiveCompleteHouseMoveTopBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id("btnCompleteMoveHouse"));
        }

        internal void AssertMoveHouseStepFiveAccountDetailsHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(accountDetailsHeaderID));
        }

        internal void AssertMoveHouseStepFiveAccountDetailsElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(elecHeaderID));
        }

        internal void AssertMoveHouseStepFiveAccountDetailsElecText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(elecTextID));
        }

        internal void AssertMoveHouseStepFiveAccountDetailsGasHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(gasHeaderID));
        }

        internal void AssertMoveHouseStepFiveAccountDetailsGasText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(gasTextID));
        }

        internal void AssertMoveHouseStepFivePropertyDetailsHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(propertyDetailsHeaderID));
        }

        internal void AssertMoveHouseStepFivePropertyDetailsPreviousHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(propertyDetailsPreviousHeaderID));
        }

        internal void AssertMoveHouseStepFivePropertyDetailsPreviousText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(propertyDetailsPreviousTextID));
        }

        internal void AssertMoveHouseStepFivePropertyDetailsNewHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(propertyDetailsNewHeaderID));
        }

        internal void AssertMoveHouseStepFivePropertyDetailsNewText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(propertyDetailsNewTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMeterHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDatePreviousPropertyHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMoveOutDateText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveOutDateTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMoveOutDate()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveOutDateID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateNewPropertyHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyMoveInDateText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInDateTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyMoveInDate()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInDateID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveOutElecHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyElecMeterReadText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveOutElecMeterReadTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyMeterRead()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveOutElecMeterReadID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInElecHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecMeterReadText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInElecMeterReadTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyElecMeterRead()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInElecMeterReadID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInGasHeaderID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasMeterReadText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInGasMeterReadTextID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyGasMeterRead()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveDateMoveInGasMeterReadID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsPreviousPropertyEditBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(editMoveOutDetailsBtnID));
        }

        internal void AssertMoveHouseStepFiveMoveDatesMeterReadsNewPropertyEditBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(editMoveInDetailsBtnID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(paymentMethodHeaderID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(paymentMethodElecHeaderID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodElecText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(paymentMethodElecTypeID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodGasHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(paymentMethodGasHeaderID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodGasText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(paymentMethodGasTypeID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodElecEditBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(editElecPaymentBtnID));
        }

        internal void AssertMoveHouseStepFivePaymentMethodGasEditBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(editGasPaymentBtnID));
        }

        internal void AssertMoveHouseStepFivePricePlanHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(pricePlanHeaderID));
        }

        internal void AssertMoveHouseStepFivePricePlanText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(pricePlanTextID));
        }

        internal void AssertMoveHouseStepFivePricePlanLink()
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//a[contains(text(), 'View Terms and Conditions here.')]"));
        }

        internal void AssertMoveHouseStepFiveCompleteHouseMoveBottomBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id("btnCompleteMoveHouse"));
        }

        internal void AssertMoveHouseStepFiveCancelBtn()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute("target", "#modal-cancel-mimo"));
        }
    }
}

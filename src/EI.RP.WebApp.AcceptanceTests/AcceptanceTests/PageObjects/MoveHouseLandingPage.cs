using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseLandingPage
    {
        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
        internal MoveHouseLandingPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseLandingPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        WebDriverWait wait;

        internal string
            movingHouseBtnID = "movingHouseTab",
            moveElecBtnID = "btnMoveElec",
            moveGasBtnID = "btnMoveGas",
            moveElecAndGasBtnID = "btnMoveElecAndGas",
            moveElecAndAddGasBtnID = "btnMoveElecAndAddGas",
            moveGasAndAddElecBtnID = "btnMoveGasAndAddElec",
            moveElecAndCloseGasBtnID = "btnMoveElecAndCloseGas",
            closeElecBtnID = "btnCloseElec",
            closeGasBtnID = "btnCloseGas",
            closeElecAndGasBtnID = "btnCloseElecAndGas";

        internal void ClickMovingHouseBtn()
        {
            driver.ClickElementEx(By.Id(movingHouseBtnID));
        }

        internal void ClickHouseMoveType(IDictionary dict, SharedVariables sharedVariables)
        {
            if (dict["ElecOnly"] == "Y")
            {
                sharedVariables.MoveElecOnly = true;
                sharedVariables.ElecMeterMoveOut = true;
                sharedVariables.ElecMeterMoveIn = true;
                AssertMoveHouseLandingPageMoveElecAndAddGasBtn();
                AssertMoveHouseLandingPageCloseElecBtn();
                ClickMoveElecBtn();
            }
            if (dict["GasOnly"] == "Y")
            {
                sharedVariables.MoveGasOnly = true;
                sharedVariables.GasMeterMoveOut = true;
                sharedVariables.GasMeterMoveIn = true;
                AssertMoveHouseLandingPageMoveGasAndAddElecBtn();
                AssertMoveHouseLandingPageCloseGasBtn();
                ClickMoveGasBtn();
            }
            if (dict["ElecAndGas"] == "Y")
            {
                sharedVariables.MoveElecAndGas = true;
                sharedVariables.ElecMeterMoveOut = true;
                sharedVariables.ElecMeterMoveIn = true;
                sharedVariables.GasMeterMoveOut = true;
                sharedVariables.GasMeterMoveIn = true;
                AssertMoveHouseLandingPageMoveElecAndCloseGasBtn();
                AssertMoveHouseLandingPageCloseElecAndGasBtn();
                ClickMoveElecAndGasBtn();
            }
            if (dict["ElecAddGas"] == "Y")
            {
                sharedVariables.MoveElecAddGas = true;
                sharedVariables.ElecMeterMoveOut = true;
                sharedVariables.ElecMeterMoveIn = true;
                sharedVariables.GasMeterMoveIn = true;
                AssertMoveHouseLandingPageMoveElecBtn();
                AssertMoveHouseLandingPageCloseElecBtn();
                ClickMoveElecAndAddGasBtn();
            }
            if (dict["GasAddElec"] == "Y")
            {
                sharedVariables.MoveGasAddElec = true;
                sharedVariables.ElecMeterMoveIn = true;
                sharedVariables.GasMeterMoveOut = true;
                sharedVariables.GasMeterMoveIn = true;
                AssertMoveHouseLandingPageMoveGasBtn();
                AssertMoveHouseLandingPageCloseGasBtn();
                ClickMoveGasAndAddElecBtn();
            }
        }

        internal void ClickMoveBtnOnPopUp(SharedVariables sharedVariables)
        {
            if (sharedVariables.MoveElecOnly)
            {
                ClickMoveHouseLandingPageMoveElecOnPopUp();
            }
            if (sharedVariables.MoveGasOnly)
            {
                ClickMoveHouseLandingPageMoveGasPopUp();
            }
            if (sharedVariables.MoveElecAndGas)
            {
                ClickMoveHouseLandingPageMoveElecAndGasPopUp();
            }
            if (sharedVariables.MoveElecAddGas)
            {
                ClickMoveHouseLandingPageMoveElecAndAddGasPopUp();
            }
            if (sharedVariables.MoveGasAddElec)
            {
                ClickMoveHouseLandingPageMoveGasAndAddElecPopUp();
            }
        }

        internal void ClickMoveElecBtn()
        {
	        driver.ClickElementEx(By.Id(moveElecBtnID));
        }

        internal void ClickMoveGasBtn()
        {
            driver.ClickElementEx(By.Id(moveGasBtnID));
        }

        internal void ClickMoveElecAndGasBtn()
        {
            driver.ClickElementEx(By.Id(moveElecAndGasBtnID));
        }

        internal void ClickMoveElecAndAddGasBtn()
        {
            driver.ClickElementEx(By.Id(moveElecAndAddGasBtnID));
        }

        internal void ClickMoveGasAndAddElecBtn()
        {
            driver.ClickElementEx(By.Id(moveGasAndAddElecBtnID));
        }

        // Popup Button clicks
        internal void ClickMoveHouseLandingPageMoveElecOnPopUp()
        {
            _sharedPageFunctions.
                ClickElement(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both-action"));
        }

        internal void ClickMoveHouseLandingPageMoveGasPopUp()
        {
            _sharedPageFunctions.
                ClickElement(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both-action"));
        }

        internal void ClickMoveHouseLandingPageMoveElecAndGasPopUp()
        {
            _sharedPageFunctions.
                ClickElement(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both-action"));
        }

        internal void ClickMoveHouseLandingPageMoveElecAndAddGasPopUp()
        {
            _sharedPageFunctions.
                ClickElement(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-and-close-action"));
        }

        internal void ClickMoveHouseLandingPageMoveGasAndAddElecPopUp()
        {
            _sharedPageFunctions.
                ClickElement(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-and-close-action"));
        }


        internal void AssertMoveHouseLandingPage()
        {
            AssertMoveHouseLandingPageHeader();
            AssertMoveHouseLandingPageAccountInfo();
            AssertMoveHouseLandingPageOptions();
            AssertMoveHouseLandingPageModals();
        }

        internal void AssertMoveHouseLandingPageModals()
        {
            AssertMoveHouseLandingPageModalMoveBoth();
            AssertMoveHouseLandingPageModalMoveAndClose();
            AssertMoveHouseLandingPageModalCloseBoth();
            AssertMoveHouseLandingPagePopUpCancelBtn();
            AssertMoveHouseLandingPagePopUpCloseCrossBtn();
        }

        internal void AssertMoveHouseLandingPageHeader()
        {
            _sharedPageFunctions.IsElementPresent(
                _sharedPageFunctions.ByDataAttribute(val: "mimo-0-header"));
        }

        internal void AssertMoveHouseLandingPageAccountInfo()
        {
            _sharedPageFunctions.IsElementPresent(
                _sharedPageFunctions.ByDataAttribute(val: "mimo-0-address-box"));
        }

        internal void AssertMoveHouseLandingPageOptions()
        {
            _sharedPageFunctions.IsElementPresent(
                _sharedPageFunctions.ByDataAttribute(val: "mimo-0-options"));
        }

        internal void AssertMoveHouseLandingPageMoveElecBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveElecBtnID));
        }

        internal void AssertMoveHouseLandingPageMoveGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveGasBtnID));
        }

        internal void AssertMoveHouseLandingPageMoveElecAndGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveElecAndGasBtnID));
        }

        internal void AssertMoveHouseLandingPageMoveElecAndAddGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveElecAndAddGasBtnID));
        }

        internal void AssertMoveHouseLandingPageMoveGasAndAddElecBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveGasAndAddElecBtnID));
        }

        internal void AssertMoveHouseLandingPageMoveElecAndCloseGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(moveElecAndCloseGasBtnID));
        }

        internal void AssertMoveHouseLandingPageCloseElecBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(closeElecBtnID));
        }

        internal void AssertMoveHouseLandingPageCloseGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(closeGasBtnID));
        }

        internal void AssertMoveHouseLandingPageCloseElecAndGasBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(closeElecAndGasBtnID));
        }

        internal void AssertMoveHouseLandingPageModalMoveBoth()
        {
            _sharedPageFunctions
                .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both"));
        }

        internal void AssertMoveHouseLandingPageModalMoveAndClose()
        {
            _sharedPageFunctions
                .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-and-close"));
        }

        internal void AssertMoveHouseLandingPageModalCloseBoth()
        {
            _sharedPageFunctions
                .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-close-both"));
        }

        internal void AssertMoveHouseLandingPagePopUpCancelBtn()
        {
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both-cancel"));
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-and-close-cancel"));
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-close-both-cancel"));
        }

        internal void AssertMoveHouseLandingPagePopUpCloseCrossBtn()
        {
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-both-close"));
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-move-and-close-close"));
            _sharedPageFunctions.
                IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-0-modal-close-both-close"));
        }
    }
}

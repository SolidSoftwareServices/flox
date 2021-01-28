using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class MoveHouseSteps : BaseStep
    {

        private MoveHouseLandingPage _moveHouseLandingPage => new MoveHouseLandingPage(shared.Driver.Value);
        private MoveHouseStepOne _moveHouseStepOne => new MoveHouseStepOne(shared.Driver.Value);
        private MoveHouseStepTwo _moveHouseStepTwo => new MoveHouseStepTwo(shared.Driver.Value);
        private MoveHouseStepThree _moveHouseStepThree => new MoveHouseStepThree(shared.Driver.Value);
        private MoveHouseStepFour _moveHouseStepFour => new MoveHouseStepFour(shared.Driver.Value);
        private MoveHouseStepFive _moveHouseStepFive => new MoveHouseStepFive(shared.Driver.Value);

        SharedPageFunctions sharedPageFunctions => new SharedPageFunctions(shared.Driver.Value);
        SharedVariables sharedVariables = new SharedVariables();

        public MoveHouseSteps(SingleTestContext shared) : base(shared)
        {
        }

        public void WhenClickMovingHouseBtn()
        {
            
            _moveHouseLandingPage.ClickMovingHouseBtn();
            _moveHouseLandingPage.AssertMoveHouseLandingPage();
        }

        public void WhenChooseHouseMoveType(IDictionary dict)
        {
            
            _moveHouseLandingPage.ClickHouseMoveType(dict, sharedVariables);
        }
        
        public void WhenClickMoveBtnOnPopUp()
        {
            _moveHouseLandingPage.ClickMoveBtnOnPopUp(sharedVariables);
            _moveHouseStepOne.AssertMoveHouseStepOnePage(sharedVariables);
        }

        public void WhenEnterMoveOutDetails(IDictionary dict)
        {
            _moveHouseStepOne.PickDateFromCalendar();
            _moveHouseStepOne.ClickRadioYesBtn();
            _moveHouseStepOne.AssertMoveHouseStepOneNewOccupantContent();
            _moveHouseStepOne.MoveHouseStepOneEnterMPRNReads(dict, sharedVariables);
            _moveHouseStepOne.ClickConfirmPermissionCheckbox();
            _moveHouseStepOne.ClickConfirmDetailsCheckbox();
            _moveHouseStepOne.ClickTermsAndConditionsCheckbox();
        }
        
        public void WhenClickNextNewMPRNGPRN()
        {
            _moveHouseStepOne.ClickNextNewMPRNGPRNBtn();
        }

        public void WhenEnterMPRNAndGRPN(IDictionary dict)
        {
            _moveHouseStepTwo.AssertMoveHouseStepTwoEnterMPRNGPRNPage(sharedVariables);
            _moveHouseStepTwo.MoveHouseStepTwoEnterMPRNGPRN(dict, sharedVariables);
        }
        
        public void WhenClickSubmitNewMPRNGPRN()
        {
            _moveHouseStepTwo.ClickSubmitNewPRN(sharedVariables);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            _moveHouseStepTwo.AssertMoveHouseStepTwoConfirmNewAddressPage(sharedVariables);
        }
        
        public void WhenClickContinueBtnOnConfirmNewAddressPage()
        {
            _moveHouseStepTwo.ClickContinueOnConfirmAddressBtn();
        }
        
        public void WhenEnterNewPropertyDetails(IDictionary dict)
        {
            _moveHouseStepThree.AssertMoveHouseStepThreePage(sharedVariables);
            _moveHouseStepThree.PickDateFromCalendar();
            _moveHouseStepThree.MoveHouseStepThreeEnterData(dict,sharedVariables);
            _moveHouseStepThree.ClickConfirmDetailsCheckbox();
            _moveHouseStepThree.ClickTermsAndConditionsCheckbox();
        }
        
        public void WhenClickNextPaymentOptionsBtn()
        {
            _moveHouseStepThree.ClickNextPaymentOptionsBtn();
        }
        
        public void WhenClickSetUpNewDDOnChoosePaymentPage()
        {
            _moveHouseStepFour.AssertMoveHouseStepFourChoosePaymentOptionPage(sharedVariables);
            _moveHouseStepFour.ClickUseDDForBothCheckbox();
            _moveHouseStepFour.ClickSetUpNewDDBtn();
        }
        
        public void WhenEnterDDDetailsOnElecDDSetUpPage(IDictionary dict)
        {
            _moveHouseStepFour.EnterDDElecDetails(dict);
            _moveHouseStepFour.ClickAuthorizeCheckboxElec();
        }
        
        public void WhenClickContinueToGasDDSetUp()
        {
            _moveHouseStepFour.ContinueToGasDDSetupBtn();
        }
        
        public void WhenClickSkipDDSetUpOnGasDDSetUpPage()
        {
            
            _moveHouseStepFour.ClickSkipSetupGasDDBtn();
        }
        
        public void WhenClickYesOnSkipGasDDSetUpPopUp()
        {
            _moveHouseStepFour.ClickYesImSurePopUpBtn();
        }
        
        public void ThenReviewScreenShouldBeDisplayed()
        {
            _moveHouseStepFive.AssertMoveHouseStepFivePage(sharedVariables);
            _moveHouseStepFive.ClickMoveHouseStepFivePropertyDetailsNewEditBtn();
            _moveHouseStepTwo.AssertMoveHouseStepTwoSubHeader(sharedVariables);
        }
    }
}

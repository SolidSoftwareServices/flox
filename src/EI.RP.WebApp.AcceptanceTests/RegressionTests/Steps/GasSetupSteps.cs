using OpenQA.Selenium;
using System;
using System.Threading;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;
using System.Collections;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using GasAccountConfirmAddressPage = EI.RP.WebApp.RegressionTests.PageObjects.GasAccountConfirmAddressPage;
using GasAccountPaymentPage = EI.RP.WebApp.RegressionTests.PageObjects.GasAccountPaymentPage;
using GasAccountSetupPage = EI.RP.WebApp.RegressionTests.PageObjects.GasAccountSetupPage;
using PlanPage = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects.PlanPage;

namespace EI.RP.WebApp.UITestAutomation
{
    public class GasSetupSteps : BaseStep
    {

        public GasSetupSteps(SingleTestContext shared) : base(shared)
        { }
        private PlanPage planPage => new PlanPage(shared.Driver.Value);
        private MyAccountsPage myAccountsPage => new MyAccountsPage(shared.Driver.Value);
        private GasAccountSetupPage gasAccountSetupPage => new GasAccountSetupPage(shared.Driver.Value);

        public void WhenClickAddGasButtonOnCTA()
        {
            planPage.ClickAddGasCTA();
            GasAccountSetupPage gasAccountSetupPage = new GasAccountSetupPage(shared.Driver.Value);
            gasAccountSetupPage.AssertGasAccountSetupPage();
        }

        public void WhenFillInGassAccountSetup(IDictionary dict)
        {  
            if (dict.ContainsKey("GPRN"))
            {
                gasAccountSetupPage.EnterGPRN(dict["GPRN"]);
            }
            if (dict.ContainsKey("Meter Reading"))
            {
                gasAccountSetupPage.EnterMeterReading(dict["Meter Reading"]);
            }
            if (dict.ContainsKey("Details Check"))
            {
                gasAccountSetupPage.ClickDetailsCheckbox(dict["Details Check"]);
            }
            if (dict.ContainsKey("General Terms Check"))
            {
                gasAccountSetupPage.ClickGeneralTermsCheckbox(dict["General Terms Check"]);
            }
            if (dict.ContainsKey("Debt Flag Check"))
            {
                gasAccountSetupPage.ClickDebtFlagCheckbox(dict["Debt Flag Check"]);
            }
            if (dict.ContainsKey("Price Plan Check"))
            {
                gasAccountSetupPage.ClickPricePlanCheckbox(dict["Price Plan Check"]);
            }
        }

        public void WhenClickSubmitOnGasAccountSetup()
        {
            gasAccountSetupPage.ClickSubmitBtn();
        }

        internal void ThenErrorDetails()
        {
            gasAccountSetupPage.ErrorDetails();
        }

        public void WhenClickYesConfirmAddressPage()
        {
            GasAccountConfirmAddressPage gasAccountConfirmAddressPage = new GasAccountConfirmAddressPage(shared.Driver.Value);
            gasAccountConfirmAddressPage.AssertGasAccountConfirmAddressPage();
            gasAccountConfirmAddressPage.ClickYesBtn();
        }

        public void WhenChooseGasAccountPayment()
        {
            GasAccountPaymentPage gasAccountPaymentPage = new GasAccountPaymentPage(shared.Driver.Value);
            gasAccountPaymentPage.AssertGasAccountPaymentPage();
            gasAccountPaymentPage.GasAccPaymentTickUseExistingDDCheckbox();
            Thread.Sleep(2000);
            gasAccountPaymentPage.GasAccPaymentClickGoBackBtn();
        }

        internal void ThenErrorTerms()
        {
            gasAccountSetupPage.ErrorTerms();
        }

        internal void ThenErrorDebt()
        {
            gasAccountSetupPage.ErrorDebt();
        }

        internal void ThenErrorPricePlan()
        {
            gasAccountSetupPage.ErrorPricePlan();
        }

        internal void ThenErrorGPRNAlpha()
        {
            gasAccountSetupPage.ErrorGPRNAlpha();
        }

        internal void ThenErrorMeterReadingAlpha()
        {
            gasAccountSetupPage.ErrorMeterReadingAlpha();
        }

        internal void ThenErrorGPRNLong()
        {
            gasAccountSetupPage.ErrorGPRNLong();  
        }

        internal void ThenErrorGPRNShort()
        {
            gasAccountSetupPage.ErrorGPRNShort();
        }

        internal void ThenErrorGPRNEmpty()
        {
            gasAccountSetupPage.ErrorGPRNEmpty();
        }

        internal void ThenErrorMeterReadingEmpty()
        {
            gasAccountSetupPage.ErrorMeterReadingEmpty();
        }
    }
}

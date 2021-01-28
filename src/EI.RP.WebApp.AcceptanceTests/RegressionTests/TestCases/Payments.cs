
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;

namespace EI.RP.WebApp.UITestAutomation
{
	class Payments : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
        MakePaymentSteps map => new MakePaymentSteps(Context);
        PaymentsSteps p => new PaymentsSteps(Context);
        BillingPreferencesSteps bp => new BillingPreferencesSteps(Context);

        [Test]
		[Category("regression")]
        public void VerifyEditDirectDebit()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.EditDd);
            Context.WhenClickBillsPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickEditDirectDebitOnPlanPage();
            bp.WhenEnterIntoTheIBANField("IE62AIBK93104777372010");
            bp.WhenEnterNameIntoTheNameOnBankAccountField("Joe Bloggs");
            bp.WhenTickTermsAndConditionsOnDirectDebitSettingsPage();
            bp.WhenClickUpdateDetailsBtn();
            bp.ThenConfirmationScreenShouldBeDisplayed();
        }
/*
        [Test]
        [Category("regression")]
        public void VerifyPaperlessBillingPromptNo()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.editDD);
            Context.WhenClickBillsPaymentsNavButton();
           // Context.WhenClickPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickPaperlessBillingOff();
            bp.WhenClickNoPaperlessBilling();
        }

        [Test]
        [Category("regression")]
        public void VerifyPaperlessBillingPromptYes()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.editDD);
            Context.WhenClickBillsPaymentsNavButton();
            // Context.WhenClickPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickPaperlessBillingOff();
            bp.WhenClickYesPaperlessBilling();
            bp.WhenClickPaperlessBillingOn();
        }
        */


        [Test]
        [Category("regression")]
        public void VerifyEditDirectDebitErrorIBAN()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.EditDd);
            Context.WhenClickBillsPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickEditDirectDebitOnPlanPage();
            bp.WhenEnterIntoTheIBANField("11111111111111111111111");
            bp.WhenEnterNameIntoTheNameOnBankAccountField("Joe Bloggs");
            bp.WhenTickTermsAndConditionsOnDirectDebitSettingsPage();
            bp.WhenClickUpdateDetailsBtn();
            bp.ThenErrorIBAN();
        }
        [Test]
        [Category("regression")]
        public void VerifyEditDirectDebitErrorName()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.EditDd);
            Context.WhenClickBillsPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickEditDirectDebitOnPlanPage();
            bp.WhenEnterIntoTheIBANField("IE62AIBK93104777372010");
            bp.WhenEnterNameIntoTheNameOnBankAccountField("11111");
            bp.WhenTickTermsAndConditionsOnDirectDebitSettingsPage();
            bp.WhenClickUpdateDetailsBtn();
            bp.ThenErrorName();
        }
        [Test]
        [Category("regression")]
        public void VerifyEditDirectDebitErrorTerms()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.EditDd);
            Context.WhenClickBillsPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            bp.WhenClickEditDirectDebitOnPlanPage();
            bp.WhenEnterIntoTheIBANField("IE62AIBK93104777372010");
            bp.WhenEnterNameIntoTheNameOnBankAccountField("Joe Bloggs");
            bp.WhenClickUpdateDetailsBtn();
            bp.ThenErrorTerms();
        }
        [Test]
        [Category("regression")]
        public void VerifyEqualizerDirectDebitSetup()
        {
            var account = EnvironmentSet.Inputs.EqualizerEligable;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            p.WhenClickBillsAndPaymentsOptionsButton();
            p.WhenCLickMoreAboutEqualMonthlyPayments();
            p.WhenClickSetUpEqualizer();
            p.WhenClickSetUpDirectDebit();
            bp.WhenEnterIntoTheIBANField("IE62AIBK93104777372010");
            bp.WhenEnterNameIntoTheNameOnBankAccountField("Joe Bloggs");
            bp.WhenTickTermsAndConditionsOnDirectDebitSettingsPage();
        }
      
		
	}
}

using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class ContactUsFormSteps : BaseStep
    {
        public ContactUsFormSteps(SingleTestContext shared) : base(shared)
        {
        }

		private DashBoardPage dashboardPage => new DashBoardPage(shared.Driver.Value.Instance);
		private ContactUsFormPage contactUsFormPage => new ContactUsFormPage(shared.Driver.Value.Instance);

        public void WhenClickContactUsInTopNavigationBar()
        {
            dashboardPage.ClickContactUsForm();
            contactUsFormPage.AssertContactUsFormBeforeSelectQueryType();
        }
        
        
        public void WhenSelectAccountFromAccountDropDownField()
        {
            contactUsFormPage.SelectAccountFromDrpDown();
        }
        
        public void WhenSelectTypeOfQueryFromDropDown(queryTypes type)
        {
            contactUsFormPage.SelectQueryFromDrpDown(type);
            if (type == queryTypes.additionalAccount)
            {
                contactUsFormPage.AssertContactUsFormAfterSelectQueryType();
            }
        }

        public void WhenEnterAccountNumberOnContactUsPageAs(string s)
        {
            contactUsFormPage.EnterAccountNumber(s);
        }
        
        public void WhenEnterLastDigitsOfMPRNGPRNAs(string s)
        {
            contactUsFormPage.EnterMPRN(s);
        }

        public void WhenEnterSubjectAs(string s)
        {
            contactUsFormPage.EnterSubject(s);
        }

        public void WhenEnterQueryAs(string s)
        {
            contactUsFormPage.EnterQueryText(s);
        }
        
        public void WhenClickSubmitQueryButton()
        {
            contactUsFormPage.ClickContactUsFormSubmitBtn();
        }
        
        
        public void ThenDisplayQueryConfirmationScreen()
        {
            contactUsFormPage.AssertContactUsConfirmationScreen();
        }

        internal void ThenErrorAccountNumberEmpty()
        {
            contactUsFormPage.errorAccountNumberEmpty();
        }

        internal void ThenErrorAccountNumberAlpha()
        {
            contactUsFormPage.errorAccountNumberAlpha();
        }

        internal void ThenErrorAccountNumberShort()
        {
            contactUsFormPage.errorAccountNumberShort();
        }

        internal void ThenErrorMPRNorGPRNAlpha()
        {
            contactUsFormPage.errorMPRNorGPRNAlpha();
        }

        internal void ThenErrorMPRNorGPRNShort()
        {
            contactUsFormPage.errorMPRNorGPRNShort();
        }

        internal void ThenErrorMPRNorGPRNEmpty()
        {
            contactUsFormPage.errorMPRNorGPRNEmpty();
        }

        internal void ThenErrorSubjectEmpty()
        {
            contactUsFormPage.errorSubjectEmpty();
        }

        internal void ThenErrorQueryEmpty()
        {
            contactUsFormPage.errorQueryEmpty();
        }

        internal void ThenErrorQueryLonger1900()
        {
            contactUsFormPage.errorQueryLonger1900();
        }
    }
}

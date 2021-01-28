
using NUnit.Framework;
using System.Threading.Tasks;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
using queryTypes = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables.queryTypes;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
	class Contact : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
        ContactUsSteps cu => new ContactUsSteps(Context);
        [Test]
		[Category("regression")]
        public void VerifyContactUsAdditionalAccount()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenDisplayQueryConfirmationScreen();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsMeterReadQuery()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.meterReadQuery);
            Thread.Sleep(2000);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenDisplayQueryConfirmationScreen();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsBillsPaymentsQuery()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.billOrPaymentQuery);
            Thread.Sleep(2000);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenDisplayQueryConfirmationScreen();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsOtherQuery()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.other);
            Thread.Sleep(2000);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenDisplayQueryConfirmationScreen();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorAccountEmpty()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorAccountNumberEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorAccountAlpha()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs("90002143a");
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorAccountNumberAlpha();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorAccountShort()
        {
            ContactUsFormSteps cu = new ContactUsFormSteps(Context);

            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs("90002143");
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorAccountNumberShort();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorMPRNorGPRNAlpha()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs("07346a");
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorMPRNorGPRNAlpha();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorMPRNorGPRNShort()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs("07346");
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorMPRNorGPRNShort();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorMPRNorGPRNEmpty()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorMPRNorGPRNEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorSubjectEmpty()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorSubjectEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorQueryEmpty()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorQueryEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorQuery1901()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("Hello, everyone! This is the LONGEST TEXT EVER! I was inspired by the various other longest texts ever on the internet, and I wanted to make my own. So here it is! This is going to be a WORLD RECORD! This is actually my third attempt at doing this. The first time, I didnt save it. The second time, the Neocities editor crashed. Now Im writing this in Notepad, then copying it into the Neocities editor instead of typing it directly in the Neocities editor to avoid crashing. It sucks that my past two attempts are gone now. Those actually got pretty long. Not the longest, but still pretty long. I hope this one wont get lost somehow. Anyways, lets talk about WAFFLES! I like waffles. Waffles are cool. Waffles is a funny word. Theres a Teen Titans Go episode called Waffles where the word Waffles is said a hundred-something times. Its pretty annoying. Theres also a Teen Titans Go episode about Pig Latin. Dont know what Pig Latin is? Its a language where you take all the consonants before the first vowel, move them to the end, and add -ay to the end. If the word begins with a vowel, you just add -way to the end. For example, Waffles becomes Afflesway. Ive been speaking Pig Latin fluently since the fourth grade, so it surprised me when I saw the episode for the first time. I speak Pig Latin with my sister sometimes. Its pretty fun. I like speaking it in public so that everyone around us gets confused. Thats never actually happened before, but if it ever does, twill be pretty funny. By the way, twill is a word I invented recently, and its a contraction of it will. I really hope it gains popularity in the near future, because twill is WAY more fun than saying itll. Itll is too boring. Nobody likes boring. This is nowhere near being the longest text ever, but eventually it will be! I might still be writing this a decade later, who knows? But right now, its not very long. But Ill ju1111111");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenErrorQueryLonger1900();
        }
        [Test]
        [Category("regression")]
        public void VerifyContactUsAdditionalAccountErrorQuery1899()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            Context.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            Thread.Sleep(2000);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("Hello, everyone! This is the LONGEST TEXT EVER! I was inspired by the various other longest texts ever on the internet, and I wanted to make my own. So here it is! This is going to be a WORLD RECORD! This is actually my third attempt at doing this. The first time, I didnt save it. The second time, the Neocities editor crashed. Now Im writing this in Notepad, then copying it into the Neocities editor instead of typing it directly in the Neocities editor to avoid crashing. It sucks that my past two attempts are gone now. Those actually got pretty long. Not the longest, but still pretty long. I hope this one wont get lost somehow. Anyways, lets talk about WAFFLES! I like waffles. Waffles are cool. Waffles is a funny word. Theres a Teen Titans Go episode called Waffles where the word Waffles is said a hundred-something times. Its pretty annoying. Theres also a Teen Titans Go episode about Pig Latin. Dont know what Pig Latin is? Its a language where you take all the consonants before the first vowel, move them to the end, and add -ay to the end. If the word begins with a vowel, you just add -way to the end. For example, Waffles becomes Afflesway. Ive been speaking Pig Latin fluently since the fourth grade, so it surprised me when I saw the episode for the first time. I speak Pig Latin with my sister sometimes. Its pretty fun. I like speaking it in public so that everyone around us gets confused. Thats never actually happened before, but if it ever does, twill be pretty funny. By the way, twill is a word I invented recently, and its a contraction of it will. I really hope it gains popularity in the near future, because twill is WAY more fun than saying itll. Itll is too boring. Nobody likes boring. This is nowhere near being the longest text ever, but eventually it will be! I might still be writing this a decade later, who knows? But right now, its not very long. But ");
            cu.WhenClickSubmitQueryButton();
            Thread.Sleep(2000);
            cu.ThenDisplayQueryConfirmationScreen();
            //cu.ThenErrorQueryLonger1900();
        }


	}
}


using NUnit.Framework;
using System.Threading.Tasks;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
using queryTypes = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables.queryTypes;
using System.Threading;
using System.Collections.Generic;

using System;
using NUnit.Framework.Interfaces;
using System.Net.Http;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = NUnit.Framework.TestContext;
using System.Diagnostics;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
	class Register : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
        RegisterSteps r => new RegisterSteps(Context);

        [Test]
        [Category("regression")]
        public void VerifyValidRegister()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenSentToCheckInboxPage();
        }
        [Test]
        [Category("regression")]
        public void VerifyInvalidMPRNWithoutAccountRegister()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN("542212");
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenSentToCheckInboxPage();
        }
        [Test]
        [Category("regression")]
        public void VerifyAccountNumberAlreadyUsed()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber("903537019");
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenSentToCheckInboxPage();
        }

        [Test]
        [Category("regression")]
        public void VerifyAccountNumberInformationModal()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            r.WhenClickAccountNumberFieldInformationButton();
            r.ThenAccountNumberPopupModalShouldAppear();
            r.WhenClickTheGoBackButton();
            r.ThenShouldBeSentToRegisterPage();
        }
        [Test]
        [Category("regression")]
        public void VerifyMPRNInformationModal()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            r.WhenClickMPRNFieldInformationButton();
            r.ThenMPRNPopupModalShouldAppear();
            r.WhenClickTheGoBackButton();
            r.ThenShouldBeSentToRegisterPage();
        }

        [Test]
        [Category("regression")]
        public void VerifyAccountNumberInformationModalX()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            r.WhenClickAccountNumberFieldInformationButton();
            r.ThenAccountNumberPopupModalShouldAppear();
            r.WhenClickTheModalXButton();
            r.ThenShouldBeSentToRegisterPage();
        }

        [Test]
        [Category("regression")]
        public void VerifyMPRNInformationModalX()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            r.WhenClickMPRNFieldInformationButton();
            r.ThenMPRNPopupModalShouldAppear();
            r.WhenClickTheModalXButton();
            r.ThenShouldBeSentToRegisterPage();
        }

        [Test]
        [Category("regression")]
        public void VerifyMPRNErrorInvalid()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorInvalidMPRN();
        }

        [Test]
        [Category("regression")]
        public void VerifyMPRNErrorAlpha()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN("6a7706");
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorInvalidMPRN();
        }

        [Test]
        [Category("regression")]
        public void VerifyMPRNErrorShort()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN("67706");
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorShortMPRN();
        }
        [Test]
        [Category("regression")]
        public void VerifyMPRNErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN("");
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorEmptyMPRN();
        }

        [Test]
        [Category("regression")]
        public void VerifyMPRNErrorLong()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN("6770666");
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorLongMPRN();
        }

        [Test]
        [Category("regression")]
        public void VerifyAccountNumberErrorLong()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber("9030053577");
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorLongAccountNumber();
        }

        [Test]
        [Category("regression")]
        public void VerifyAccountNumberErrorShort()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber("90300535");
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorShortAccountNumber();
        }
        [Test]
        [Category("regression")]
        public void VerifyAccountNumberErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber("");
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorEmptyAccountNumber();
        }
        [Test]
        [Category("regression")]
        public void VerifyAccountNumberErrorAlpha()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber("9a3005357");
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
           r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorInvalidAccountNumber();
        }

        [Test]
        [Category("regression")]
        public void VerifyEmailErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail("");
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorEmptyEmail();
        }
        [Test]
        [Category("regression")]
        public void VerifyDOBErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB("29/02");
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorEmptyDOB();
        }
        [Test]
        [Category("regression")]
        public void VerifyTermsErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenConfirmThatIAmAnAccountHolder();
            r.WhenClickRegister();
            r.ThenErrorEmptyTermsAndConditions();
        }
        [Test]
        [Category("regression")]
        public void VerifyAccountHolderErrorEmpty()
        {
            var acc = EnvironmentSet.Inputs.RegisterAccount;
            r.GivenNavigateToTheRegisterPage();
            
            
            r.WhenEnterAccountNumber(acc["AccountNumber"]);
            r.WhenEnterLastSixDigitsOfMPRN(acc["MPRN"]);
            r.WhenEnterEmail(acc["Email"]);
            r.WhenEnterDOB(acc["DOB"]);
            r.WhenEnterPhoneNumber(acc["Phone"]);
            r.WhenAcceptTheTermsAndConditions();
            r.WhenClickRegister();
            r.ThenErrorEmptyAccountHolder();
        }

        /*
         Feature: Register
	In order to avail of smart metering services
	As a user
	I want to be told the register for a smart meter account


#----------------------------------------------------------------------------
@ignore
Scenario: Verify the content of the Information modal within the MPRN input field

	Given Navigate to the Register Page
	When Click MPRN field information button
	Then Modal header should say xxxxxxxxxxxxx
	And Modal text should say xxxxxxx
	And There should be an x in the top right corner
	And There should be a go back button
	#Then Popup modal should appear
    

#----------------------------------------------------------------------------
@ignore
Scenario: Verify input field Last 6 digits of MPRN cannot be populated without an already registered account being associated with it

	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 29/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid MPRN

#----------------------------------------------------------------------------
@ignore
Scenario: Verify when the Information button within the Last 6 digits of MPRN input field is clicked, a new pop up modal is displayed

	Given Navigate to the Register Page
	When Click account number field information button
	Then MPRN Popup modal should appear

#----------------------------------------------------------------------------
@ignore
Scenario: Verify the X button on the Information modal within the MPRN input field works

	Given Navigate to the Register Page
	When Click MPRN field information button
	And Click the modal x button
	Then Should be sent to Register Page

#----------------------------------------------------------------------------
@ignore
Scenario: Verify in the input field Date of Birth, the input field Year is a text field where field cannot be set to a date in the future

	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter future DOB
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error invalid DOB

#----------------------------------------------------------------------------
@ignore
Scenario: Verify in the  input field  Date of Birth, the Date of Birth will only be accepted for 29th February if it falls on a Leap year

	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 29/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page

#----------------------------------------------------------------------------
@ignore
Scenario: Verify the checkbox for I confirm that I am the account holder  can be unselected

	Given Navigate to the Register Page
	Then unfinished

#----------------------------------------------------------------------------
@ignore
Scenario: Verify when the user who already has an online account tries to sign up, they are not able to

	Given Navigate to the Register Page
	Then unfinished

#----------------------------------------------------------------------------
@ignore
Scenario: Verify Register for Online Account

	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter DOB details with 29 of February on a leap year
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 29/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter short dd Date of Birth details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 1/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter short mm Date of Birth details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/2/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page 

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty First Name details
	
	Given Navigate to the Register Page
	When Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	#Then Register button should be greyed out
	And Click Register
	Then Error Empty First Name
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty Last Name details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error Empty Last Name
	
#----------------------------------------------------------------------------

@ignore
Scenario: Verify enter empty Account Number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	#Then Register button should be greyed out
	And Click Register
	Then Error Empty Account Number
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter alphabetical Account Number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 90300535a
	And Enter last six digits of MPRN as 667706
	And Enter Account Number as abcdefghi
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid Account Number	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty MPRN details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	#Then Register button should be greyed out
	And Click Register
	Then Error Empty MPRN	
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter alphabetical MPRN details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 123abc
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid MPRN
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty email details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error Empty Email
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty phone number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	#Then Register button should be greyed out
	And Click Register
	Then Error invalid MPRN
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter alphabetical Phone Number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 123abc123
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid Phone Number
	
#----------------------------------------------------------------------------

@ignore
Scenario: Verify enter empty Date of Birth details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error Empty DOB
	

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty Terms and Conditions details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error Empty Terms And Conditions
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter empty Account Holder
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Click Register
	#Then Register button should be greyed out
	Then Error Empty Account Holder
	

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long First Name details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe456789012345678901234567890123456789012345678901
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error invalid First Name
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long Last Name details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs789012345678901234567890123456789012345678901
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error invalid Last Name
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long Account Number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357111111
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid Account Number
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long MPRN details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 6677067
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long email details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmailllllllllllllllllllllllllllllllllllllllllllllllllll.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error Long Email
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter long phone number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456789101234567890123456789012345678
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page	
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter bracketed phone number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as (086)8612345
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter spaced phone number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 0 8 6 1 23456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Should be sent to Check Inbox Page
	
#----------------------------------------------------------------------------

@ignore
Scenario: Verify enter short Account Number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 0123456789
	And Enter last six digits of MPRN as 667706
	And Enter Email as Joe.Bloggs@gmail.com on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error Short Account Number
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter short MPRN details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 66770
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error Short MPRN

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter short phone number details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 0861234
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	##Then Register button should be greyed out
	Then Error Short Phone Number	

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter short yyyy Date of Birth details
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/200
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error invalid DOB

#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter dd Date of Birth details outside 1 and 31
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 32/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	#Then Register button should be greyed out
	Then Error invalid DOB
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify enter mm Date of Birth details outside 1 and 12
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/13/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error invalid DOB
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify Terms and Conditions Button
	
	Given Navigate to the Register Page
	When Click the Terms and Conditions Link
	Then Should be sent to Terms and Conditions Page
	
#----------------------------------------------------------------------------
@ignore
Scenario: Verify Register With Existing Email
	
	Given Navigate to the Register Page
	When Enter First Name as Joe
	And Enter Last Name as Bloggs
	And Enter Account Number as 903005357
	And Enter last six digits of MPRN as 667706
	And Enter Email as duelfuel@esb.ie on Register page
	And Enter Phone Number as 086123456
	And Enter DOB as 01/02/2000
	And Accept the Terms and Conditions
	And Confirm that I am an account holder
	And Click Register
	Then Error as Email Already In Use

#----------------------------------------------------------------------------
@ignore
Scenario: Verify Register From Smart Meter Hub Page
	
	Given Navigate to Smart Meter Hub Page
	When Click Sign Up for Smart Services
	Then Should be sent to Register Page
	And Register Page Text as In just a few minutes you'll have access to Smart Services dashboard

#----------------------------------------------------------------------------
@ignore
Scenario: Verify Register From Log In
	
	Given Navigate to Smart Meter Hub Page
	When Click Log Into Smart Services
	And Click Create Account link
	Then Should be sent to Register Page
	And Register Page Text as Set up your online account

#----------------------------------------------------------------------------
@ignore
Scenario: Verify the text of the Privacy notice box
	Given Navigate to Smart Meter Hub Page
	When Click Log Into Smart Services
	And Click Create Account link
	Then Should be sent to Register Page
	And Privacy notice should say Electric Ireland requires the below information to create and administer you account. The data controller is the Electricity Supply Board, trading as Electric Ireland. Please refer to our Privacy Notice.

#----------------------------------------------------------------------------
@ignore
Scenario: Verify the privacy notice link
	Given Navigate to Smart Meter Hub Page
	When Click Log Into Smart Services
	And Click Create Account link
	And Click Privacy Notice Link
	Then Should be sent to EI Privacy Notice and Cookies Policy Page
         */

      
	
	}
}

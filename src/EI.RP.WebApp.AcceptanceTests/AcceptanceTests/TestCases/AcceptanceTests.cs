using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;
using queryTypes = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables.queryTypes;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.TestCases
{
	[Parallelizable(ParallelScope.Children)]
	public class AcceptanceTests : ResidentialPortalBrowserFixture
    {

	    [RetryOnError]
	    [Test, Order(1)]
	    [Category("smoke")]
        public async Task Usage()
        {
            UsageSteps u = new UsageSteps(Context);
            await Context.NavigateToTestStart(EnvironmentSet.Inputs.Usage);
            u.ThenUsageShouldBeDisplayed();
            //u.WhenClickKWH();
            u.WhenClickPrevYear();
            u.WhenClickNextYear();
            //u.WhenClickCompareYears();
            //u.WhenClickCompareNow();
        }
        
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(10)]
        public async Task MeterReading()
        {
            SubmitMeterReadingSteps mr = new SubmitMeterReadingSteps(Context);

            await Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReading);
            Context.WhenClickMeterReadingNavButton();
            mr.WhenEnterIntoTheMeterReadingInputField("123456");
            mr.WhenClickSubmitMeterReadingButton();
            mr.ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed();
        }
       
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(2)]
        public async Task AddGas()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };
            AddGasSteps ag = new AddGasSteps(Context);

            await Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();

            ag.WhenClickAddGasButtonOnCTA();
            ag.WhenFillInGassAccountSetup(addGasFormDetails);
            ag.WhenClickSubmitOnGasAccountSetup();
            ag.WhenClickYesConfirmAddressPage();
            ag.WhenChooseGasAccountPayment();
        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(3)]
        public async Task BillsAndPayments()
        {
            BillsAndPaymentsSteps bp = new BillsAndPaymentsSteps(Context);
			await Context.NavigateToTestStart(EnvironmentSet.Inputs.EditDd);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickBillingPreferencesButton();
            bp.WhenClickEditDirectDebitOnBillingAndPaymentsOptionsPage();
            bp.WhenEnterIntoTheIBANField("IE62AIBK93104777372010"); 
            bp.WhenEnterNameIntoTheNameOnBankAccountField("Joe Bloggs");
            bp.WhenTickTermsAndConditionsOnDirectDebitSettingsPage();
            bp.WhenClickUpdateDetailsBtn();
            bp.ThenConfirmationScreenShouldBeDisplayed();
        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(4)]
        public async Task ContactUs()
        {
            ContactUsFormSteps cu = new ContactUsFormSteps(Context);

            await Context.NavigateToTestStart(EnvironmentSet.Inputs.ContactUs);
            cu.WhenClickContactUsInTopNavigationBar();
            cu.WhenSelectAccountFromAccountDropDownField();
            cu.WhenSelectTypeOfQueryFromDropDown(queryTypes.additionalAccount);
            cu.WhenEnterAccountNumberOnContactUsPageAs(EnvironmentSet.Inputs.ContactUsAddAccount["Account"]);
            cu.WhenEnterLastDigitsOfMPRNGPRNAs(EnvironmentSet.Inputs.ContactUsAddAccount["MPRN"]);
            cu.WhenEnterSubjectAs("Add Account");
            cu.WhenEnterQueryAs("This is a query");
            cu.WhenClickSubmitQueryButton();
            cu.ThenDisplayQueryConfirmationScreen();
        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(8)]
        public async Task EnergyServices()
        {
            EnergyServicesSteps es = new EnergyServicesSteps(Context);

            await Context.NavigateToTestStart(EnvironmentSet.Inputs.EnergyServices);
            es.ThenContactUsShouldBeDisplayed();
            es.ThenEnergyServicesAccountDetailsShouldBeDisplayed();
        }
        [RetryOnError]
        [Category("smoke")]  
        [Test, Order(6)]
        public void AgentLogin()
        {
            AgentLoginSteps al = new AgentLoginSteps(Context);

            Context.GivenOnTheAgentLoginScreen();
            al.WhenLoginToTheAgentPortal(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterUserNameInUsernameField(EnvironmentSet.Inputs.AdminSearch["Email"]);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnDeRegistrationBtn();
            
            al.WhenClickOnCancelBtnOnDeRegistrationPopup();
            
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(7)]
        public void Refund()
        {
	        RefundSteps refund = new RefundSteps(Context);

	        Context.NavigateToTestStartStayOnMyAccounts(EnvironmentSet.Inputs.Refund);
            refund.WhenClickSubmitRefundRequestButton(EnvironmentSet.Inputs.Refund);
            refund.WhenEnterAdditionalInformation("I am some addition information");
            refund.WhenClickSubmitOnRefundForm();
            refund.ThenShouldBeSentToRefundConfirmationPage();
        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(5)]
        public async Task MakeAPayment()
        {
            MakeAPaymentSteps map = new MakeAPaymentSteps(Context);
            BillsAndPaymentsSteps bp = new BillsAndPaymentsSteps(Context);

            await Context.NavigateToTestStart(EnvironmentSet.Inputs.MakeAPayment);    
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs("1");
            map.WhenClickSubmitOnPayDifferentAmount();

        }
        [RetryOnError]
        [Category("smoke")]
        [Test, Order(-1)]
        public async Task MoveHouse()
        {
            IDictionary<string, string> moveTypeDetails = new Dictionary<string, string>()
            {
                { "ElecOnly" ,  "N" },
                { "GasOnly" , "N" },
                { "ElecAndGas" , "N"},
                { "ElecAddGas"  , "Y" },
                { "GasAddElec"  , "N" }
            };
            IDictionary<string, string> moveOutDetails = new Dictionary<string, string>()
            {
                { "ElecMeterRead1" , "12345"} ,
                { "ElecMeterRead2" , "1234"} ,
                { "GasMeterRead" , "1234"} ,
                { "24HrMeter" , "Y"} ,
                { "DayMeter" , "N"} ,
                { "NightMeter" , "N"} ,
                { "NSHMeter" , "N"} ,
                { "Name" , "Karl Heinz"},
                { "Phone","0871234567"}
            };

            IDictionary<string, string> mprnGprnDetails = new Dictionary<string, string>()
            {
                { "MPRN" , "10000320276"} ,
                { "GPRN" , "0047702"}
            };

            IDictionary<string, string> propertyDetails = new Dictionary<string, string>()
            {
                { "ContactPhone" , "0871234568" },
                { "24HrMeter" , "Y" },
                { "DayMeter" , "N" },
                { "NightMeter" , "N" },
                { "NSHMeter" , "N"} ,
                { "ElecMeterRead1" , "1233"} ,
                { "ElecMeterRead2" , "1232"} ,
                { "GasMeterRead" , "1234"}
            };
            IDictionary<string, string> ddDetails = new Dictionary<string, string>()
            {
                { "IBAN" , "IE91AIBK93516600556065"} ,
                { "Name" , "Karl Gustav"}
            };
            MoveHouseSteps mh = new MoveHouseSteps(Context);

           await  Context.NavigateToTestStart(EnvironmentSet.Inputs.MoveHouse);
            Context.WhenClickMoveHouseNavBtn();
            mh.WhenChooseHouseMoveType(moveTypeDetails);
            mh.WhenClickMoveBtnOnPopUp();
            mh.WhenEnterMoveOutDetails(moveOutDetails);
            mh.WhenClickNextNewMPRNGPRN();
            mh.WhenEnterMPRNAndGRPN(mprnGprnDetails);
            mh.WhenClickSubmitNewMPRNGPRN();
            mh.WhenClickContinueBtnOnConfirmNewAddressPage();
            mh.WhenEnterNewPropertyDetails(propertyDetails);
            mh.WhenClickNextPaymentOptionsBtn();
            mh.WhenClickSetUpNewDDOnChoosePaymentPage();
            mh.WhenEnterDDDetailsOnElecDDSetUpPage(ddDetails);
            mh.WhenClickContinueToGasDDSetUp();
            mh.WhenClickSkipDDSetUpOnGasDDSetUpPage();
            mh.WhenClickYesOnSkipGasDDSetUpPopUp();
            mh.ThenReviewScreenShouldBeDisplayed();
        }
      
    }
}



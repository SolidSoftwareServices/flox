
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
	class AddGas : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
        GasSetupSteps gs => new GasSetupSteps(Context);

        AddGasSteps ag => new AddGasSteps(Context);
        [Test]
		[Category("regression")]
        public void VerifyAddGas()
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
            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();

            ag.WhenClickAddGasButtonOnCTA();
            ag.WhenFillInGassAccountSetup(addGasFormDetails);
            ag.WhenClickSubmitOnGasAccountSetup();
            ag.WhenClickYesConfirmAddressPage();
            ag.WhenChooseGasAccountPayment();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorDetails()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "1234"},
                { "Details Check", "False"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorDetails();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorTerms()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "False"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorTerms();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorDebt()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "False"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorDebt();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorPricePlan()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "False"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorPricePlan();
        }

        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorMeterReadingEmpty()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", ""},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorMeterReadingEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorGPRNEmpty()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorGPRNEmpty();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorGPRNShort()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorGPRNShort();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorGPRNLong()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "54258591" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorGPRNLong();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorGPRNAlpha()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "542585a" },
                { "Meter Reading", "1234"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorGPRNAlpha();
        }
        [Test]
        [Category("regression")]
        public void VerifyAddGasErrorMeterReadingAlpha()
        {
            IDictionary<string, string> addGasFormDetails = new Dictionary<string, string>()
            {
                { "GPRN", "5425859" },
                { "Meter Reading", "123a"},
                { "Details Check", "True"},
                { "General Terms Check", "True"},
                { "Debt Flag Check", "True"},
                { "Price Plan Check", "True"}
            };

            Context.NavigateToTestStart(EnvironmentSet.Inputs.AddGas);
            Context.WhenClickPlanNavButton();
            gs.WhenClickAddGasButtonOnCTA();
            gs.WhenFillInGassAccountSetup(addGasFormDetails);
            gs.WhenClickSubmitOnGasAccountSetup();
            gs.ThenErrorMeterReadingAlpha();
        }
       
	
	}
}

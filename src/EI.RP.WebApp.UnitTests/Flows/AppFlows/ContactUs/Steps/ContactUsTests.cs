using System.Collections.Generic;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.ContactUs.Infrastructure.StepsDataBuilder;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.ContactUs.Steps
{
    [TestFixture]
    internal class ContactUsTests
    {
        private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.ContactUs.Steps.ContactUs, ResidentialPortalFlowType>
            NewScreenTestConfigurator(DomainFacade domainFacade = null)
        {
            //creates the domain layer facade
            if (domainFacade == null) domainFacade = new DomainFacade();
            //creates a flo test configurator
            return new FlowScreenTestConfigurator<WebApp.Flows.AppFlows.ContactUs.Steps.ContactUs, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
                //Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
                .WithStub(domainFacade.SessionProvider)
                .WithStub(domainFacade.QueryResolver)
                .WithStub(domainFacade.CommandDispatcher);
        }

        public static IEnumerable<TestCaseData> QueryType_Cases(string caseName)
        {
            yield return new TestCaseData(ContactQueryType.AddAdditionalAccount).SetName($"{caseName} AddAdditionalAccount QueryType");
            yield return new TestCaseData(ContactQueryType.BillOrPayment).SetName($"{caseName} BillOrPayment QueryType");
            yield return new TestCaseData(ContactQueryType.MeterRead).SetName($"{caseName} MeterRead QueryType");
            yield return new TestCaseData(ContactQueryType.Other).SetName($"{caseName} Other QueryType");
        }

        [TestCaseSource(nameof(QueryType_Cases), new object[] { nameof(ItCanChangeQueryType) })]
		public void ItCanChangeQueryType(ContactQueryType contactQueryType)
        {
            //Arrange
            var appUserConfigurator = new AppUserConfigurator(new DomainFacade());
            appUserConfigurator.AddElectricityAccount(configureDefaultDevice: false);
            appUserConfigurator.Execute();

            var rootDataBuilder = new RootDataBuilder(appUserConfigurator);
            var stepDataBuilder = new ContactUsStepDataBuilder(appUserConfigurator);

            var stepData = stepDataBuilder.Create();
            stepData.SelectedQueryType = contactQueryType;

            NewScreenTestConfigurator(appUserConfigurator.DomainFacade)
                .NewEventTestRunner(stepData)
                .WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
                //act
                .WhenEvent(WebApp.Flows.AppFlows.ContactUs.Steps.ContactUs.StepEvent.QueryChanged)

                //flow step assertions 
               .ThenTheResultStepIs(ContactUsStep.ContactUs);
        }

        [TestCaseSource(nameof(QueryType_Cases), new object[] { nameof(ItCanSubmitMessage) })]
		public void ItCanSubmitMessage(ContactQueryType contactQueryType)
        {
            //Arrange

            var appUserConfigurator = new AppUserConfigurator(new DomainFacade());
            appUserConfigurator.AddElectricityAccount(configureDefaultDevice: false);
            appUserConfigurator.Execute();

            var rootDataBuilder = new RootDataBuilder(appUserConfigurator);
            var stepDataBuilder = new ContactUsStepDataBuilder(appUserConfigurator);

            var stepData = stepDataBuilder.Create();
            stepData.SelectedQueryType = contactQueryType;

            NewScreenTestConfigurator(appUserConfigurator.DomainFacade)
                .NewEventTestRunner(stepData)
                .WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
                //act
                .WhenEvent(WebApp.Flows.AppFlows.ContactUs.Steps.ContactUs.StepEvent.SubmitQuery)
                //flow step assertions 
                .ThenTheResultStepIs(ContactUsStep.ShowContactUsStatusMessage);
        }


		public static IEnumerable<TestCaseData> ItHasCorrectStringTypedValue_Cases()
		{
			yield return new TestCaseData(ContactQueryType.AddAdditionalAccount, "Add an additional account").SetName($"ItHasCorrectStringTypedValue {ContactQueryType.AddAdditionalAccount} QueryType");
			yield return new TestCaseData(ContactQueryType.BillOrPayment, "Bill or payment query").SetName($"ItHasCorrectStringTypedValue {ContactQueryType.BillOrPayment} QueryType");
			yield return new TestCaseData(ContactQueryType.MeterRead, "Meter read query").SetName($"ItHasCorrectStringTypedValue {ContactQueryType.MeterRead} QueryType");
			yield return new TestCaseData(ContactQueryType.Other, "Other").SetName($"ItHasCorrectStringTypedValue {ContactQueryType.Other} QueryType");
		}

		[TestCaseSource(nameof(ItHasCorrectStringTypedValue_Cases))]
		public void ItHasCorrectStringTypedValue(ContactQueryType contactQueryType, string expectedValue)
		{
			Assert.IsTrue(contactQueryType.ToString().Equals(expectedValue));
		}
	}
}

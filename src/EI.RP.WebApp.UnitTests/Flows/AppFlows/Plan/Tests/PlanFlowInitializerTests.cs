using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.Plan.Steps;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Tests
{
	[TestFixture]
	internal class PlanFlowInitializerTests
	{
		private FlowInitializerTestConfigurator<PlanFlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			bool anonymousUser = false)
		{
			return NewTestConfigurator(anonymousUser
				? new DomainFacade()
				: new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
		}

		private FlowInitializerTestConfigurator<PlanFlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			DomainFacade domainFacade)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowInitializerTestConfigurator<PlanFlowInitializer, ResidentialPortalFlowType>(
					domainFacade.ModelsBuilder)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		[Test]
		[Theory]
		public async Task CanAuthorize(bool anonymousUser)
		{
			var expected = !anonymousUser;
			var actual = NewTestConfigurator(anonymousUser).Adapter.Authorize(false);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FlowTypeIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.Plan, NewTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		[Theory]
		public async Task Can_InitializeContext(bool mainAccountIsElectricityAccount, bool withDuelFuel)
		{
			var cfg = ConfigureDomain(mainAccountIsElectricityAccount, withDuelFuel);

			var mainAccountNumber = mainAccountIsElectricityAccount
				? cfg.ElectricityAccount().Model.AccountNumber
				: cfg.GasAccount().Model.AccountNumber;

			NewTestConfigurator(cfg.DomainFacade)
				.NewInitializationRunner()
				.WithInput<IPlanInput>(new PlanInput
				{
					AccountNumber = mainAccountNumber
				})
				.Run()
				.AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual))
				.AssertStepData<PlanFlowInitializer.RootScreenModel>(actual =>
				{
					Assert.AreEqual(mainAccountNumber, actual.AccountNumber);
				});
		}


		[Test]
		public async Task ItHandles_ErrorEvent()
		{
			NewTestConfigurator()
				.NewEventTestRunner()
				.WhenEvent(ScreenEvent.ErrorOccurred)
				.TheResultStepIs(PlanStep.ShowFlowGenericError);
		}

		[Test]
		public async Task ItHandles_StartEvent()
		{
			var rootStepData = new PlanFlowInitializer.RootScreenModel();

			NewTestConfigurator()
				.NewEventTestRunner()
				.GivenTheStepDataIs(rootStepData)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs(PlanStep.MainScreen);
		}


		private static AppUserConfigurator ConfigureDomain(bool mainAccountIsElectricityAccount, bool withDuelFuel)
		{
			var cfg = new AppUserConfigurator()
				.SetValidSession();
			var mainAccount = mainAccountIsElectricityAccount
				? (CommonElectricityAndGasAccountConfigurator) cfg.AddElectricityAccount()
				: cfg.AddGasAccount();
			if (withDuelFuel)
			{
				if (mainAccount.AccountType == ClientAccountType.Electricity)
					cfg.AddGasAccount(duelFuelSisterAccount: cfg.ElectricityAccount());
				else
					cfg.AddElectricityAccount(duelFuelSisterAccount: cfg.GasAccount());
			}

			cfg.Execute();
			return cfg;
		}
	}
}
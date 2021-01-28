using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.Plan.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Infrastructure.Builders
{
	class PlanDetailsDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		public PlanDetailsDataBuilder(AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}
		public MainScreen.ScreenModel LastCreated { get; private set; }
		public MainScreen.ScreenModel Create()
		{
			return LastCreated= _domainConfigurator.DomainFacade.ModelsBuilder.Build<MainScreen.ScreenModel>()
				.With(x => x.AccountNumber, _rootDataBuilder.AccountNumber)
				.Create();

		}
	}
}
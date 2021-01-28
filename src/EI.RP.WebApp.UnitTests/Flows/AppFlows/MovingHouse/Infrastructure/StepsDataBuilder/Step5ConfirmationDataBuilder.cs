using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step5ConfirmationDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		public Step5ConfirmationDataBuilder(
	        AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}

		public Step5ConfirmationScreen.ScreenModel LastCreated { get; private set; }

		public Step5ConfirmationScreen.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step5ConfirmationScreen.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
				.Create();
		}
    }
}
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step5ReviewAndCompleteDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		public Step5ReviewAndCompleteDataBuilder(
	        AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}

		public Step5ReviewAndComplete.ScreenModel LastCreated { get; private set; }

		public Step5ReviewAndComplete.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step5ReviewAndComplete.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
				.Without(x => x.MovingDatesMeterReadingInfoReadingsInfo)
				.Without(x => x.HasValidationErrors)
				.Create();
		}
    }
}
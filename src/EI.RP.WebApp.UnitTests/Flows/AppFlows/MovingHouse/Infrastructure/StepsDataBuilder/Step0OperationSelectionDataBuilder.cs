using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step0OperationSelectionDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		public Step0OperationSelectionDataBuilder(AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}
		public Step0OperationSelection.ScreenModel LastCreated { get; private set; }
		public Step0OperationSelection.ScreenModel Create()
		{
			return LastCreated= _domainConfigurator.DomainFacade.ModelsBuilder.Build<Step0OperationSelection.ScreenModel>()
				.With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
				.Create();

		}
	}
}
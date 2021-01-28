using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step3InputMoveInPropertyDetailsDataBuilder
    {
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

        public Step3InputMoveInPropertyDetailsDataBuilder(
	        AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}

		public Step3InputMoveInPropertyDetails.ScreenModel LastCreated { get; private set; }

		public Step3InputMoveInPropertyDetails.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step3InputMoveInPropertyDetails.ScreenModel>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
                .Without(x => x.UserMeterInputFields)
                .Create();
		}
    }
}
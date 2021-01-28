using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step1InputCloseAccountDetailsDataBuilder
    {
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;
		private readonly Step0OperationSelectionDataBuilder _step0Builder;

		public Step1InputCloseAccountDetailsDataBuilder(AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder, Step0OperationSelectionDataBuilder step0Builder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
			_step0Builder = step0Builder;
		}

		public Step1InputCloseAccountDetails.ScreenModel LastCreated { get; private set; }

		public Step1InputCloseAccountDetails.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step1InputCloseAccountDetails.ScreenModel>()
				.With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
				.With(x => x.UserMeterInputFields, _rootDataBuilder.LastCreated.UserMeterInputFields)
				.Without(x => x.HasValidationErrors)
				.Create();
		}
	}
}
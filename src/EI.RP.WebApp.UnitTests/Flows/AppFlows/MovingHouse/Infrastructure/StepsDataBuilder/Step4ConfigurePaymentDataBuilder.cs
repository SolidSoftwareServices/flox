using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder
{
	class Step4ConfigurePaymentDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;
		private readonly RootDataBuilder _rootDataBuilder;

		public Step4ConfigurePaymentDataBuilder(
	        AppUserConfigurator domainConfigurator,
			RootDataBuilder rootDataBuilder)
		{
			_domainConfigurator = domainConfigurator;
			_rootDataBuilder = rootDataBuilder;
		}

		public Step4ConfigurePayment.StepData LastCreated { get; private set; }

		public Step4ConfigurePayment.StepData Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder
				.Build<Step4ConfigurePayment.StepData>()
                .With(x => x.MovingHouseType, _rootDataBuilder.MovingType)
				.Create();
		}
    }
}
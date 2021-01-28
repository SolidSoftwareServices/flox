using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public RootDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}

		public SmartActivationFlowInitializer.StepsSharedData LastCreated { get; private set; }

		public SmartActivationFlowInitializer.StepsSharedData Create()
		{
			var fixture = _domainConfigurator.DomainFacade.ModelsBuilder;
			return (LastCreated = fixture.Build<SmartActivationFlowInitializer.StepsSharedData>()
				.Create());
		}
	}
}
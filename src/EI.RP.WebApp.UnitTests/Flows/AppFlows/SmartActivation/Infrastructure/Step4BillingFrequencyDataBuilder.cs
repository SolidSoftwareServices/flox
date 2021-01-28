using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class Step4BillingFrequencyDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public Step4BillingFrequencyDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Step4BillingFrequency.ScreenModel LastCreated { get; private set; }
		public Step4BillingFrequency.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder.Build<Step4BillingFrequency.ScreenModel>()
				.Create();
		}
	}
}
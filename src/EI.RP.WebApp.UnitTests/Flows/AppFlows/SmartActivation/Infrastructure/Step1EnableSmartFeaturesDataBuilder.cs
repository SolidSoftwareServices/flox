using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class Step1EnableSmartFeaturesDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public Step1EnableSmartFeaturesDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Step1EnableSmartFeatures.ScreenModel LastCreated { get; private set; }
		public Step1EnableSmartFeatures.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder.Build<Step1EnableSmartFeatures.ScreenModel>()
				.Create();
		}
	}
}
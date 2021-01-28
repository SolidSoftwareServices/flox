using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class ConfirmationDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public ConfirmationDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Confirmation.ScreenModel LastCreated { get; private set; }
		public Confirmation.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder.Build<Confirmation.ScreenModel>()
				.Create();
		}
	}
}
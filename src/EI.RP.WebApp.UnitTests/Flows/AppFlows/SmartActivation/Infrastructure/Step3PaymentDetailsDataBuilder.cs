using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure
{
	class Step3PaymentDetailsDataBuilder
	{
		private readonly AppUserConfigurator _domainConfigurator;

		public Step3PaymentDetailsDataBuilder(AppUserConfigurator domainConfigurator)
		{
			_domainConfigurator = domainConfigurator;
		}
		public Step3PaymentDetails.ScreenModel LastCreated { get; private set; }
		public Step3PaymentDetails.ScreenModel Create()
		{
			return LastCreated = _domainConfigurator.DomainFacade.ModelsBuilder.Build<Step3PaymentDetails.ScreenModel>()
				.Create();
		}
	}
}
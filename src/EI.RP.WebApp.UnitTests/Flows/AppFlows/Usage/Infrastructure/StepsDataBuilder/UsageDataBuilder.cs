using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Usage.Infrastructure.StepsDataBuilder
{
	class UsageDataBuilder
    {
		private readonly AppUserConfigurator _appUserConfigurator;
        private readonly string _accountNumber;

        public UsageDataBuilder(AppUserConfigurator appUserConfigurator, string accountNumber )
		{
			_appUserConfigurator = appUserConfigurator;
            _accountNumber = accountNumber;
        }

		public WebApp.Flows.AppFlows.Usage.Steps.Usage.ScreenModel Create()
		{
            var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;

            var result = fixture.Build<WebApp.Flows.AppFlows.Usage.Steps.Usage.ScreenModel>()
				.With(x => x.AccountNumber, _accountNumber)
                .Create();
			return result;
		}
	}
}
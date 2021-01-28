using System;
using System.Linq;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.Usage.Steps;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Usage.Infrastructure.StepsDataBuilder
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _appUserConfigurator;

		public RootDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			_appUserConfigurator = appUserConfigurator;
		}

		public UsageFlowInitializer.RootScreenModel Create(bool mainAccountIsElectricityAccount)
		{
            var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;

            var mainAccountConfigurator = mainAccountIsElectricityAccount ?
                (CommonElectricityAndGasAccountConfigurator)_appUserConfigurator.ElectricityAccount() :
                (CommonElectricityAndGasAccountConfigurator)_appUserConfigurator.GasAccount();

            var accountConfigurator =
				(CommonElectricityAndGasAccountConfigurator) _appUserConfigurator.ElectricityAccount() ??
				_appUserConfigurator.GasAccount();


            var result = fixture.Build<UsageFlowInitializer.RootScreenModel>()
				.With(x => x.AccountNumber, mainAccountConfigurator.Model.AccountNumber)
                .Create();
			return result;
		}
	}
}
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.Steps;
using System;
using System.Linq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.CompetitionEntry.Infrastructure.StepsDataBuilder
{
    class RootDataBuilder
    {
        private readonly AppUserConfigurator _appUserConfigurator;

        public RootDataBuilder(AppUserConfigurator appUserConfigurator)
        {
            _appUserConfigurator = appUserConfigurator;
        }

        public CompetitionEntryFlowInitializer.RootScreenModel Create()
        {
            if (_appUserConfigurator.Accounts.Count() != 1)
            {
                throw new NotSupportedException();
            }

            var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;
            var accountConfigurator =
                (CommonElectricityAndGasAccountConfigurator)_appUserConfigurator.ElectricityAccount() ??
                _appUserConfigurator.GasAccount();
            var result = fixture.Build<CompetitionEntryFlowInitializer.RootScreenModel>()
                .With(x => x.AccountNumber, accountConfigurator.Model.AccountNumber)
                .Create();
            return result;
        }
    }
}

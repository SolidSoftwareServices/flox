using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.Steps;
using System;
using System.Linq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.ContactUs.Infrastructure.StepsDataBuilder
{
    class RootDataBuilder
    {
        private readonly AppUserConfigurator _appUserConfigurator;

        public RootDataBuilder(AppUserConfigurator appUserConfigurator)
        {
            _appUserConfigurator = appUserConfigurator;
        }

        public ContactUsFlowInitializer.RootScreenModel Create()
        {
            if (_appUserConfigurator.Accounts.Count() != 1)
            {
                throw new NotSupportedException();
            }

            var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;
            var accountConfigurator =
                (CommonElectricityAndGasAccountConfigurator)_appUserConfigurator.ElectricityAccount() ??
                _appUserConfigurator.GasAccount();
            var result = fixture.Build<ContactUsFlowInitializer.RootScreenModel>()
                .With(x => x.AccountNumber, accountConfigurator.Model.AccountNumber)
                .Create();
            return result;
        }
    }
}
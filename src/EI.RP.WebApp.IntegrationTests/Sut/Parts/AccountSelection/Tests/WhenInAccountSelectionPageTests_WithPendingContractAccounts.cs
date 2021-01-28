using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
    [TestFixture]
    class WhenInAccountSelectionPageTests_WithPendingContractAccounts : WebAppPageTests<AccountSelectionPage>
    {
        protected AppUserConfigurator _userConfig;
        protected AccountSelectionPage _sut;

        protected override async Task TestScenarioArrangement()
        {
            _userConfig = App.ConfigureUser("a@A.com", "test");
            _userConfig.AddElectricityAccount(isContractPending: true);
            _userConfig.AddElectricityAccount(isContractPending: true);
            _userConfig.AddElectricityAccount(opened: true);
            _userConfig.Execute();
            await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role);
            _sut = App.CurrentPageAs<AccountSelectionPage>();
        }

        [Test]
        public void CanSeeUserAccountsWithPendingContracts()
        {
            CollectionAssert.AreEquivalent(_userConfig.Accounts.Where(x => x.IsOpen || x.ContractStatus == ContractStatusType.Pending)
                .Select(x => x.AccountNumber), _sut.ReadAccountNumbers());
        }
    }
}






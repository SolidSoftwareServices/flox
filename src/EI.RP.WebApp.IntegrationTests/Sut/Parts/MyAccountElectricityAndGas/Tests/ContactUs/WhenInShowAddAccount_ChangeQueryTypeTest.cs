using AutoFixture;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DomainServices.Commands.Users.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.ContactUs
{
    [TestFixture]
    internal class WhenInShowAddAccount_ChangeQueryTypeTest : WhenInContactUsPageTest
    {
        protected override async Task TestScenarioArrangement()
        {
            await base.TestScenarioArrangement();

            var sut = Sut;
            Sut = (await sut.SelectQueryType_AddAccount()).CurrentPageAs<ContactUsPage>();
        }

        [Test]
        public async Task CanSeeComponentItemsAndSubmitAfterErrors()
        {
            Assert.IsNotNull(Sut.AdditionalAccountComponent);
            Assert.IsNotNull(Sut.AccountNumberInput);
            Assert.IsNotNull(Sut.MprnInput);


            Sut.AccountNumberInput.Value = "900021432";
            Sut.MprnInput.Value = "111111";
            Sut.SubjectInput.Value = Fixture.Create<string>();
            Sut.QueryInput.Value = Fixture.Create<string>();

            var cmd = new UserContactRequest(UserConfig.Accounts.Single().Partner, Sut.AccountNumberInput.Value,
                Sut.MprnInput.Value, Sut.SubjectInput.Value, Sut.QueryInput.Value, ContactQueryType.AddAdditionalAccount);

            App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(ResidentialDomainError.Invalid_MPRN));

            var errorPage = (await App.ClickOnElement(Sut.SubmitQueryButton)).CurrentPageAs<ContactUsPage>();
            //the domain text could be different from the one we present
            Assert.AreEqual(ResidentialDomainError.Invalid_MPRN.ErrorMessage, errorPage.ErrorMessage.TextContent);


            cmd = new UserContactRequest(UserConfig.Accounts.Single().Partner, Sut.AccountNumberInput.Value,
                Sut.MprnInput.Value, Sut.SubjectInput.Value, Sut.QueryInput.Value, ContactQueryType.AddAdditionalAccount);
            Sut.MprnInput.Value = "111110";

            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
            await App.ClickOnElement(Sut.SubmitQueryButton);
            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

        }
    }
}
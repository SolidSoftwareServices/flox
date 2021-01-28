using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Users.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.ContactUs
{
    [TestFixture]
    internal class WhenInShowMeterRead_ChangeQueryTypeTest : WhenInContactUsPageTest
    {
        protected override async Task TestScenarioArrangement()
        {
            await base.TestScenarioArrangement();

            Sut = (await Sut.ChangeQueryType(ContactQueryType.MeterRead)).CurrentPageAs<ContactUsPage>();
        }

        [Test]
        public async Task CanSubmitQuery()
        {
            Assert.IsNull(Sut.AccountNumberInput);
			Assert.IsNull(Sut.MprnInput);
            Assert.IsNotNull(Sut.FaqMeterReadComponent);

            Sut.SubjectInput.Value = Fixture.Create<string>();
            Sut.QueryInput.Value = Fixture.Create<string>();

           var cmd = new UserContactRequest(UserConfig.Accounts.Single().Partner, Sut.SelectedAccount.Value,
	           null, Sut.SubjectInput.Value, Sut.QueryInput.Value, ContactQueryType.MeterRead);

           App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
           var confirmationPage = await App.ClickOnElement(Sut.SubmitQueryButton);
           confirmationPage.CurrentPageAs<ContactUsConfirmationPage>();
           App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

        }
    }
}
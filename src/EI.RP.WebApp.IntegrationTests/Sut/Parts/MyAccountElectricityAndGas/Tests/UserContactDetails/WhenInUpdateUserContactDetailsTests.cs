using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.UserContact;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.UserContactDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.UserContactDetails
{
	[TestFixture]
    internal class WhenInUpdateUserContactDetailsTests : MyAccountCommonTests<UserContactDetailsPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount();
            UserConfig.Execute();
            var accountInfo = UserConfig.Accounts.First();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut = (await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToContactDetails(accountInfo.AccountNumber))
                .CurrentPageAs<UserContactDetailsPage>();
        }

        [Test]
        public async Task VisualItemsArePresented()
        {
            Assert.IsNotNull(Sut.ContactDetailsHeading);
            Assert.AreEqual("Contact Details", Sut.ContactDetailsHeading.TextContent);

            Assert.IsNotNull(Sut.PrivacyComponent);

			Assert.IsNotNull(Sut.ContactEmailLablel);
            Assert.AreEqual("Contact Email", Sut.ContactEmailLablel.TextContent);

            Assert.IsNotNull(Sut.AlternativePhoneNumberLabel);
            Assert.AreEqual("Alternative Phone (optional)", Sut.AlternativePhoneNumberLabel.TextContent);

            Assert.IsNotNull(Sut.PrimaryPhoneNumberInput);
            Assert.IsNotNull("Primary phone number", Sut.PrimaryPhoneNumberLabel.TextContent);

            Assert.IsNotNull(Sut.LoginEmail);
            Assert.IsNotNull(UserConfig.UserName, Sut.LoginEmail.TextContent);

            Assert.IsNotNull(Sut.SaveChangesButton);
            Assert.AreEqual("Save Changes", Sut.SaveChangesButton.TextContent);
        }

        [Test]
        [TestCase("0892365179")]
        [TestCase("")]
		public async Task CanUpdateContactDetails(string newAltPhone)
        {
            var userContactInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().UserContactDetails;
            var newEmail = Fixture.Create<EmailAddress>().ToString();
            var newPrimaryPhone = "0892365178";

            Sut.AlternativePhoneNumberInput.Value = newAltPhone;
            Sut.PrimaryPhoneNumberInput.Value = newPrimaryPhone;
            Sut.ContactEmailInput.Value = newEmail;

            var previousEmail = userContactInfo.ContactEMail;

            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;

            var updateUserContactCommand = new UpdateUserContactDetailsCommand(accountInfo.AccountNumber,
                Sut.PrimaryPhoneNumberInput.Value,
                Sut.AlternativePhoneNumberInput.Value, 
                Sut.ContactEmailInput.Value,
                 previousEmail);
            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(updateUserContactCommand);

            var page =
                (await Sut.ClickOnElement(Sut.SaveChangesButton))
                .CurrentPageAs<UserContactDetailsPage>();


            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(updateUserContactCommand);

            Assert.IsNotNull(page.SuccessMessage);
            Assert.AreEqual("Your contact details have been successfully changed.", page.SuccessMessage.TextContent);
        }
    }
}

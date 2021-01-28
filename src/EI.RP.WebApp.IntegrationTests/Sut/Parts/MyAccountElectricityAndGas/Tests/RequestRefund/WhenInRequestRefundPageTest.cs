using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DomainServices.Commands.Billing.RequestRefund;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.RequestRefund;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.RequestRefund
{

    [TestFixture]
    class WhenInRequestRefundPageTest : MyAccountCommonTests<RequestRefundPage>
    {
        protected virtual bool ScenarioRequires_That_CanRefundedLatestBill { get; } = true;
        protected virtual bool ScenarioRequires_That_HasAccountCredit { get; } = true;
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(hasAccountCredit: ScenarioRequires_That_HasAccountCredit, canBeRefunded: ScenarioRequires_That_CanRefundedLatestBill)
                .WithInvoices(3);
            UserConfig.Execute();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToAccounts();
            app = await app.CurrentPageAs<AccountSelectionPage>().ToRequestRefund(0);
            Sut = app.CurrentPageAs<RequestRefundPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
            Assert.AreEqual("Account", Sut.AccountLabel.TextContent);
            Assert.AreEqual("Account Credit", Sut.AccountCreditLabel.TextContent);
            Assert.AreEqual("Additional Information", Sut.CommentLabel.TextContent);
            Assert.NotNull(Sut.AccountText);
            Assert.NotNull(Sut.AccountCreditText);
            Assert.NotNull(Sut.SubmitRequestRefund);
        }

        [Test]
        public async Task PrivacyLinkGoesToTermsInfoPage()
        {
            (await Sut.ToPrivacyNoticeViaComponent()).CurrentPageAs<TermsInfoPage>();
        }

		[Test]
		public async Task WhenCommentsExceedMaxLength_ShowsError()
		{
			var fixture = new Fixture().CustomizeFrameworkBuilders();
			//value of length 1900
			var additionalInformation = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

			var cmd = new RequestRefundCommand(fixture.Create<string>(), fixture.Create<string>(), PaymentMethodType.Manual, additionalInformation);

			await Sut.ClickOnElement(Sut.SubmitRequestRefund);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(DomainError.GeneralValidation));
		}

		[Test]
		public async Task WhenCommentIsBlank_ShowsError()
		{
			var fixture = new Fixture().CustomizeFrameworkBuilders();
			var additionalInformation = "";

			var cmd = new RequestRefundCommand(fixture.Create<string>(), fixture.Create<string>(), PaymentMethodType.Manual, additionalInformation);

			await Sut.ClickOnElement(Sut.SubmitRequestRefund);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(cmd, new DomainException(DomainError.GeneralValidation));
		}
	}
}
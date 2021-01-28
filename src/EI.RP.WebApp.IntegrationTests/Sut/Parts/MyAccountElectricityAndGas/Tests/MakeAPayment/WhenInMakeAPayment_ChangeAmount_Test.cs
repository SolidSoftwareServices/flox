using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MakeAPayment
{
    [TestFixture]
    internal class WhenInMakeAPayment_ChangeAmount_Test : WhenInMakeAPaymentPageTests<MakeAPaymentPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            await base.TestScenarioArrangement();

            var sut = ((MakeAPaymentPage_Input)Sut);
            Sut = (await sut.ClickPayDifferentAmountButton()).CurrentPageAs<MakeAPaymentPage_ChangeAmount>();
        }

        [Test, Ignore("TODO")]
        public async Task CanSeeComponentItems()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task CanCancel()
        {
            var sut = (MakeAPaymentPage_ChangeAmount)Sut;
            var inputPage = (await sut.ClickCancel()).CurrentPageAs<MakeAPaymentPage_Input>();
            Assert.NotNull(inputPage.ShowPaymentDetails.ShowPayDifferentAmountButton);
        }

        [Test]
        public async Task CanSeeChangeAmountPage()
        {
            var sut = (MakeAPaymentPage_ChangeAmount)Sut;
            Assert.NotNull(sut.InputBox);
			Assert.IsTrue(sut.InputBox.GetAttribute("inputmode").Contains("decimal"));
			Assert.NotNull(sut.ChangeAmount.SubmitButton());
            Assert.NotNull(sut.ChangeAmount.CancelButton());
        }

        [Test]
        public async Task CanSubmit()
        {
            var sut = (MakeAPaymentPage_ChangeAmount)Sut;
            sut.InputBox.Value = App.Fixture.Create<EuroMoney>().Amount.ToString();
            var inputPage = (await sut.ClickSubmit()).CurrentPageAs<MakeAPaymentPage_Input>();

            Assert.NotNull(inputPage.ShowPaymentDetails.ShowPayDifferentAmountButton);
        }

        [Test]
        public async Task WhenEmptyShowsError()
        {
            var sut = (MakeAPaymentPage_ChangeAmount)Sut;
            sut.InputBox.Value = string.Empty;
            sut = (await sut.ClickSubmit()).CurrentPageAs<MakeAPaymentPage_ChangeAmount>();
            Assert.IsTrue(sut.InputBox.ClassList.Contains("input-validation-error"));

        }

        [Test]
        public async Task WhenChangeAndCancelTheAmountIsTheLastChangedAmount()
        {
            var pageChangeAmount = (MakeAPaymentPage_ChangeAmount)Sut;

            var expected = App.Fixture.Create<EuroMoney>().Amount.Value.ToString(CultureInfo.InvariantCulture);
            while (pageChangeAmount.InputBox.Value == expected)
            {
                expected = App.Fixture.Create<EuroMoney>().Amount.Value.ToString(CultureInfo.InvariantCulture);
            }

            pageChangeAmount.InputBox.Value = expected;

            var inputPage = (await pageChangeAmount.ClickSubmit()).CurrentPageAs<MakeAPaymentPage_Input>();

            pageChangeAmount = (await inputPage.ClickPayDifferentAmountButton())
                .CurrentPageAs<MakeAPaymentPage_ChangeAmount>();
            inputPage = (await pageChangeAmount.ClickCancel()).CurrentPageAs<MakeAPaymentPage_Input>();

            Assert.IsTrue( ((EuroMoney)inputPage.ShowPaymentDetails.AmountDue).Amount.Value.ToString(CultureInfo.InvariantCulture).Contains(expected));
        }

        [Test]
        public override async Task PrivacyLinkGoesToTermsInfoPage()
        {
            var page=(await Sut.ToPrivacyNoticeViaComponent()).CurrentPageAs<TermsInfoPage>();
			Assert.IsNotNull(page);
        }
    }
}
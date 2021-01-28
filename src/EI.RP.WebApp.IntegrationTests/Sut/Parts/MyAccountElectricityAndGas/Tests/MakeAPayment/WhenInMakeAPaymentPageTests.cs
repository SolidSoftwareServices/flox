using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MakeAPayment
{
    [TestFixture]
    abstract class WhenInMakeAPaymentPageTests<TPage> : MyAccountCommonTests<TPage>
        where TPage : MakeAPaymentPage
    {
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(paymentType: PaymentType,canEstimateLatestBill: CanEstimateLatestBill)
				.WithInvoices(3);
			UserConfig.Execute();
			var residentialPortalApp =
                await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMakeAPayment()).CurrentPageAs<TPage>();
        }

		public virtual bool CanEstimateLatestBill { get; } = false;

		public virtual PaymentMethodType PaymentType { get; } = PaymentMethodType.Equalizer;

        [Ignore("test elavon callback service")]
		[Test]
		public Task CanHandleElavonResults()
		{
			throw new NotImplementedException();
		}
		[Ignore("TODO")]
		[Test]
		public Task CanPayDifferentAmount()
		{
			throw new NotImplementedException();
		}

		[Ignore("TODO")]
		[Test]
		public Task CanTryPayDifferentAmount_ThenCancel()
		{
			throw new NotImplementedException();
		}

        [Test]
        public virtual async Task PrivacyLinkGoesToTermsInfoPage()
        {
            (await Sut.ToPrivacyNoticeViaComponent()).CurrentPageAs<TermsInfoPage>();
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Usage;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders
{
	[TestFixture]
	class HeadersNavigationTests: MyAccountCommonTests<MyAccountElectricityAndGasPage>
	{
		protected override async Task TestScenarioArrangement()
		{

			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount().WithInvoices(3);
			UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

			Sut = App.CurrentPageAs<MyAccountElectricityAndGasPage>();
		}
		[Test]
		public async Task CanLogout_NavigatesToLoginPage()
		{
			(await Sut.Logout()).CurrentPageAs<LoginPage>();
		}

		[Test]
		public virtual async Task CanNavigateTo_Usage()
		{
			(await Sut.ToUsage()).CurrentPageAs<UsagePage>();
		}

		[Test, Ignore("TODO")]
		public virtual async Task CanNavigateTo_SubmitMeterReading()
		{
			throw new NotImplementedException();
			//(await Sut.ToMeterReading()).CurrentPageAs<SubmitMeterReadingsPage_Single>();
		}

		[Test]
		public virtual async Task CanNavigateTo_BillsAndPayments()
		{
			(await Sut.ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
		}
	}
}
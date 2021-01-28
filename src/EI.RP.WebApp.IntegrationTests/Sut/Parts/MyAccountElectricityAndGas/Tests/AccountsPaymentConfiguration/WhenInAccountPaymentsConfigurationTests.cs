using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
    [TestFixture]
    abstract class WhenInAccountPaymentsConfigurationTests : MyAccountCommonTests<ShowPaymentsHistoryPage>
    {
        protected override async Task TestScenarioArrangement()
        {
	        UserConfig = App.ConfigureUser("a@A.com", "test");
            var accountConfigurator = UserConfig.AddElectricityAccount(
				opened: IsOpen,
	            withPaperBill: HasPaperBill,
                paymentType: PaymentMethodType);
			
            if (HasInvoices)
            {
	            accountConfigurator.WithInvoices(3);
	            if (HasOverDueInvoice)
	            {
		            accountConfigurator.FinancialActivitiesConfiguration.WithOverDueInvoice();
	            }
			}

            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
        }

        protected void SetHasPaperBill(bool switchStatus)
        {
            HasPaperBill = switchStatus;
        }

        protected void SetIsAccountOpen(bool isOpen)
        {
	        IsOpen = isOpen;
        }

        protected void SetHasInvoices(bool hasInvoices)
        {
			HasInvoices = hasInvoices;
		}

        protected void SetHasOverDueInvoice()
        {
	        HasOverDueInvoice = true;
        }
        protected void SetPaymentMethodType(PaymentMethodType paymentMethodType)
        {
            PaymentMethodType = paymentMethodType;
        }

        protected virtual bool HasPaperBill { get; private set; } = true;
		protected virtual bool HasOverDueInvoice { get; private set; }
        protected virtual PaymentMethodType PaymentMethodType { get; private set; } = PaymentMethodType.DirectDebit;
        protected virtual bool IsOpen { get; private set; } = true;
		protected virtual bool HasInvoices { get; private set; } = true;
    }
}
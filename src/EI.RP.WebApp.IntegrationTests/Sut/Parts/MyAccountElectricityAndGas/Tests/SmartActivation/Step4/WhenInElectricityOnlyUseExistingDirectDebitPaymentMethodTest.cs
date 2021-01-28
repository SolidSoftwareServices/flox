
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step4
{
	[TestFixture]
    class WhenInElectricityOnlyUseExistingDirectDebitPaymentMethodTest : WhenInSmartActivationStep4Test
    {
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.DirectDebit;

	    protected override async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
	    {
		    step3.ExistingDirectDebitElement.ConfirmUseExistingDebit.IsChecked = true;
		    var step4 = (await step3.ClickOnElement(step3.ExistingDirectDebitElement.UseExistingDirectDebitButton))
			    .CurrentPageAs<Step4BillingFrequencyPage>();
		    return step4;
	    }
    }
}
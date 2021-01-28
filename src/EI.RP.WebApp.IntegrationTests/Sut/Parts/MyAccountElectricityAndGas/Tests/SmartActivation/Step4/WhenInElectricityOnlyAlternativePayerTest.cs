using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step4
{
	[TestFixture]
    class WhenInElectricityOnlyAlternativePayerTest : WhenInSmartActivationStep4Test
    {
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.DirectDebitNotAvailable;

	    protected override async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
	    {
		    var step4 = (await step3.ClickOnElement(step3.AlternativePayerElement.ContinueButton));
		    return step4.CurrentPageAs<Step4BillingFrequencyPage>();
	    }

		[Test]
	    public override async Task CanSeeComponents()
	    {
			Assert.IsNotNull(Sut.ContinueButton);
		    Assert.IsNotNull(Sut.BillingDayOfMonthOption);
			Assert.AreEqual("Payment will be due approximately 14 days after this date, or in line with your Credit Union fixed payment date", Sut.AlternativePayerNote.TextContent);
			Assert.IsNull(Sut.StandardPayerNote);
		}
    }
}
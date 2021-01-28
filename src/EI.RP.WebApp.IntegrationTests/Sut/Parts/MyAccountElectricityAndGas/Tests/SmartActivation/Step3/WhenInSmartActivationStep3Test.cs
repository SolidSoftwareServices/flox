using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step3
{
	[TestFixture]
    abstract class WhenInSmartActivationStep3Test : WhenInSmartActivationTest<Step3PaymentDetailsPage>
    {
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
	    protected override bool IsDualFuel => false;

	    protected override async Task<Step3PaymentDetailsPage> NavigateToCurrentStep()
	    {
		    var step1Page = App.CurrentPageAs<Step1EnableSmartFeaturesPage>();
			var step2Page = (await step1Page.NextPage(true))
				.CurrentPageAs<Step2SelectPlanPage>();

			var smartPlans = UserConfig.ElectricityAndGasAccountConfigurators.First(x => x.AccountType.IsElectricity()).SmartPlans;
			return (await step2Page.SelectPlan(smartPlans[2])).CurrentPageAs<Step3PaymentDetailsPage>();
		}
    }
}

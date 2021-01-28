using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step2
{
    [TestFixture]
    class WhenInSmartActivationStep2Test : WhenInSmartActivationTest<Step2SelectPlanPage>
    {
	    protected override bool IsDualFuel => false;
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;

	    protected override async Task<Step2SelectPlanPage> NavigateToCurrentStep()
	    {
			var step1Page = App.CurrentPageAs<Step1EnableSmartFeaturesPage>();

			return (await step1Page.NextPage(true)).CurrentPageAs<Step2SelectPlanPage>();
		}

	    [Test]
		public override async Task CanSeeComponents()
	    {
			var smartPlans = UserConfig.ElectricityAndGasAccountConfigurators.Single().SmartPlans;
			foreach (var smartPlan in smartPlans)
			{
				Sut.AssertPlan(smartPlan);
			}
		}
		
	    [Test]
		public async Task CanSeeComponents_AndSelectingNormalPlan_NavigatesToStep3()
		{
			var smartPlan = UserConfig.ElectricityAndGasAccountConfigurators.Single().SmartPlans.Last();
			Sut.AssertPlan(smartPlan);
			
			await Sut.SelectPlan(smartPlan);

			App.CurrentPageAs<Step3PaymentDetailsPage>();
		}

    }
}

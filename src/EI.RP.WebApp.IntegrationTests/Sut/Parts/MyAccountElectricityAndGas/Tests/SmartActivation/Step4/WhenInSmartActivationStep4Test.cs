using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step4
{
	[TestFixture]
	abstract class WhenInSmartActivationStep4Test : WhenInSmartActivationTest<Step4BillingFrequencyPage>
	{
		protected override bool IsDualFuel => false;

		protected override async Task<Step4BillingFrequencyPage> NavigateToCurrentStep()
		{
			var step1Page = App.CurrentPageAs<Step1EnableSmartFeaturesPage>();

			var step2Page = await NavigateToStep2(step1Page);
			var step3Page = await NavigateToStep3(step2Page);
			return await NavigateToStep4(step3Page);
		}

		protected virtual async Task<Step2SelectPlanPage> NavigateToStep2(Step1EnableSmartFeaturesPage step1)
		{
			return (await step1.NextPage(true))
				.CurrentPageAs<Step2SelectPlanPage>();
		}

		protected virtual async Task<Step3PaymentDetailsPage> NavigateToStep3(Step2SelectPlanPage step2)
		{
			var smartPlans = UserConfig.ElectricityAndGasAccountConfigurators.Single().SmartPlans;

			await step2.SelectPlan(smartPlans[0]);
			return App.CurrentPageAs<Step3PaymentDetailsPage>();
		}

		protected virtual async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
		{
			var step4 = (await step3.ClickOnElement(step3.SetupDirectDebitElementForManualPayment.SkipButton));
			return step4.CurrentPageAs<Step4BillingFrequencyPage>();
		}

		[Test]
		public virtual async Task CanContinueToStep5()
		{
			Sut.BillingFrequencyTypeEveryMonth.IsChecked = true;
			Sut.BillingDayOfMonthOption.Value = "1";
			(await Sut.ClickOnElement(Sut.ContinueButton)).CurrentPageAs<Step5SummaryPage>();
		}

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsNotNull(Sut.BillingDayOfMonthOption);
			Assert.AreEqual("Payment will be due approximately 14 days after this date", Sut.StandardPayerNote.TextContent);
			Assert.IsNull(Sut.AlternativePayerNote);
		}
	}
}
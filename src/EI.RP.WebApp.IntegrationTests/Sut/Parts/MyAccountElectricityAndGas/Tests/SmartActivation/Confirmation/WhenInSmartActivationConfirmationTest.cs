using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Confirmation
{
	[TestFixture]
    abstract class WhenInSmartActivationConfirmationTest : WhenInSmartActivationTest<ConfirmationPage>
    {
	    protected virtual BillingFrequencyType BillingFrequencyType => BillingFrequencyType.EveryMonth;
	    protected virtual int BillingDayOfMonth => 1;

		protected virtual SmartPlan SelectedPlan { get; set; } = null;

		protected override async Task<ConfirmationPage> NavigateToCurrentStep()
	    {
			var step1Page = App.CurrentPageAs<Step1EnableSmartFeaturesPage>();
			var step2Page = (await step1Page.NextPage(true)).CurrentPageAs<Step2SelectPlanPage>();
			var step3Page = await NavigateToStep3(step2Page);
			var step4Page = await NavigateToStep4(step3Page);
			var step5Page = await NavigateToStep5(step4Page);
			return await NavigateToConfirmation(step5Page);
		}

		protected virtual async Task<Step3PaymentDetailsPage> NavigateToStep3(Step2SelectPlanPage step2)
		{
			var smartPlans = UserConfig.ElectricityAndGasAccountConfigurators.FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity).SmartPlans;
			SelectedPlan = smartPlans[0];
			await step2.SelectPlan(SelectedPlan);
			return App.CurrentPageAs<Step3PaymentDetailsPage>();
		}

		protected virtual async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
		{
			var step4 = (await step3.ClickOnElement(step3.SetupDirectDebitElementForManualPayment.SkipButton));
			return step4.CurrentPageAs<Step4BillingFrequencyPage>();
		}

		protected virtual async Task<Step5SummaryPage> NavigateToStep5(Step4BillingFrequencyPage step4)
		{
			return (await step4.NextPage(BillingFrequencyType, BillingDayOfMonth)).CurrentPageAs<Step5SummaryPage>();
		}

		protected virtual async Task<ConfirmationPage> NavigateToConfirmation(Step5SummaryPage step5)
		{
			return (await step5.NextPage(true)).CurrentPageAs<ConfirmationPage>();
		}

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsNotNull(Sut.PageHeading);
			Assert.IsTrue(Sut.PageHeading.TextContent.Contains("all done"));
			Assert.IsNotNull(Sut.ThankYouWithSelectedPlan);
			Assert.AreEqual($"Thank you for signing up to {SelectedPlan.PlanName}", Sut.ThankYouWithSelectedPlan.TextContent.Trim());
			Assert.IsNotNull(Sut.WeAreDelightedElement);
			Assert.AreEqual("We are delighted to bring you our most advanced smart plan with energy and insights to help you reduce your energy consumption and costs.", Sut.WeAreDelightedElement.TextContent.Trim());
			Assert.IsNotNull(Sut.YouWillShortlyElement);
			Assert.AreEqual("You will shortly receive an email confirming your selection. In the next few days, we will update your online account to reflect this change.", Sut.YouWillShortlyElement.TextContent.Trim());
			Assert.IsNotNull(Sut.BackToAccountsLink);
		}

		[Test]
		public async Task CanNavigateBackToAccounts()
		{
			var accountPage = (await Sut.ClickOnElement(Sut.BackToAccountsLink)).CurrentPageAs<AccountSelectionPage>();
		}

		[Test]
		public override async Task CanCancel()
		{
			Assert.IsNull(Sut.CancelButton);
		}
	}
}

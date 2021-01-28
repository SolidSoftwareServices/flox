using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	EqualizerMonthlyPayments;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_EqualizerTest : WhenInPlanPageTests
	{
		protected override bool IsSmartAccount => false;

		[Test]
		public async Task CanSeeEqualizerComponents()
		{
			Assert.IsFalse(Sut.EqualiserLink?.GetAttribute("class").Contains("disabled"));
			Assert.IsTrue(Sut.EqualiserHeading?.TextContent.Equals("Equal monthly payments"));
			Assert.IsTrue(
				Sut.EqualiserText?.TextContent.Equals(
					"Spread the cost of your energy with fixed monthly Direct Debit."));
			Assert.IsTrue(Sut.EqualiserLink?.TextContent.Equals("Find out more about equal monthly payments"));

			var page = await App.ClickOnElement(Sut.EqualiserLink);
			page.CurrentPageAs<EqualizerMonthlyPaymentsPage>();
		}
	}
}
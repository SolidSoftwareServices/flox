using System.Threading.Tasks;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_EqualizerTest_WithContractPending : WhenInPlanPageTests
	{
		protected override bool IsSmartAccount => false;
		protected override bool WithIsContractPending => true;

		[Test]
		public async Task CanSeeEqualizerComponents()
		{
			Assert.IsTrue(Sut.EqualiserDisabledLink?.GetAttribute("class").Contains("disabled"));
			Assert.IsNotNull(Sut.EqualiserDisabledLink);
			Assert.IsNull(Sut.EqualiserLink);
			Assert.IsTrue(Sut.EqualiserHeading?.TextContent.Equals("Equal monthly payments"));
			Assert.IsTrue(
				Sut.EqualiserText?.TextContent.Equals(
					"Spread the cost of your energy with fixed monthly Direct Debit."));
			Assert.IsTrue(Sut.EqualiserDisabledLink?.TextContent.Equals("Find out more about equal monthly payments"));
		}
	}
}
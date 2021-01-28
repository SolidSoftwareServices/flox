using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
	internal class WhenInPlanPage_SmartMeterOnChangeToStandardPlan : WhenInPlanPageTests
	{
		protected override bool CanSmartMeterMoveToToStandardPlan => true;
		protected override bool IsSmartMeterOnChangeToStandardPlan => true;

		[Test]
		public async Task ShouldNotSeeComponents()
		{
			Assert.IsNull(Sut.SmartMeterDataHeading);
			Assert.IsNull(Sut.SmartMeterDataToggle);
			Assert.IsNull(Sut.SmartMeterDataText);
			Assert.IsNull(Sut.SmartMeterDataDowngradCheckBoxText);
		}
	}
}

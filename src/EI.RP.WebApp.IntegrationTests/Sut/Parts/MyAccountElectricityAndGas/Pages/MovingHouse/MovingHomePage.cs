using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal abstract class MovingHomePage : MyAccountElectricityAndGasPage
	{
		protected MovingHomePage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return base.IsInPage();
		}
	}
}
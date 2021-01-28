namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    class WhenInMovingHomeLandingPageDuelFuelFromGasTest : WhenInMovingHomeLandingPageDuelFuelFromElectricityTest
    {
        protected override bool FromElectricityToGas { get; } = false;
    }
}
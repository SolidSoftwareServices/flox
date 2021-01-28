using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveAndAddAccount
{
    class WhenMoveElectricityAndAddGas_ExistingPaymentIsManual: WhenMoveOneAccountAndAddOtherAccount
    {
        protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
        protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.Manual;

        protected override bool IsPRNDegistered { get; } = false;
		[Test]
        public async Task HandlesUserPath_MANUAL_ThenNEW_ShowsInputScreenHasSkipLink()
        {
            //bypassing javascript as still experimental on anglesharp and not working
            await Sut.SelectNewDirectDebitFromDialog();
            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
            Assert.IsNotNull(inputPage.SkipDirectDebitSetupLink);
        }
    }
}
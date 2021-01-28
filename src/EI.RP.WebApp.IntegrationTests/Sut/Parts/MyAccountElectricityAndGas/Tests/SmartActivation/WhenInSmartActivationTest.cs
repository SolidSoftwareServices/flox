using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation
{
	[TestFixture]
    abstract class WhenInSmartActivationTest<TPage> : MyAccountCommonTests<TPage>
        where TPage : SmartActivationPage
    {
	    protected abstract PaymentMethodType PaymentMethod {get;}
    
	    protected abstract bool IsDualFuel { get; }

	    protected override async Task TestScenarioArrangement()
	    {
		    UserConfig = App.ConfigureUser("a@A.com", "test");
		    UserConfig
			    .AddElectricityAccount(paymentType: PaymentMethod)
			    .WithElectricity24HrsDevices(RegisterConfigType.MCC01, CommsTechnicallyFeasibleValue.CTF3)
			    .WithSmartPlans();
		    if (IsDualFuel)
		    {
			    UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
		    }
		    UserConfig.Execute();
		    await App
			    .WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
		    var accountSelectionPage = App.CurrentPageAs<AccountSelectionPage>();
		    await accountSelectionPage.ToSmartActivation();
		    Sut = await NavigateToCurrentStep();
	    }

	    protected abstract Task<TPage> NavigateToCurrentStep();

		[Test]
        public abstract Task CanSeeComponents();

        [Test]
        public virtual async Task CanCancel()
        {
	        (await Sut.ClickOnElement(Sut.CancelButton)).CurrentPageAs<AccountSelectionPage>();
        }
	}
}
using System;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step1
{
	
    class WhenInElectricityOnlyTest : WhenInSmartActivationTest<Step1EnableSmartFeaturesPage>
    {
	    protected override bool IsDualFuel => false;
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;

	    protected override async Task<Step1EnableSmartFeaturesPage> NavigateToCurrentStep()
	    {
		    return App.CurrentPageAs<Step1EnableSmartFeaturesPage>();
	    }
		
		[Test]
        public override async Task CanSeeComponents()
        {
			Assert.IsNotNull(Sut.CancelButton);
			Assert.IsNotNull(Sut.InformationCollectionAuthorizedCheckbox);
			Assert.IsNotNull(Sut.EnableSmartServicesButton);
			Assert.IsNotNull(Sut.NoThanksSmartActivationLink);
			Assert.IsNotNull(Sut.MoreAboutSmartMetersLink);
			Assert.IsNotNull(Sut.SmartActivationPrivacyNoticeLink);
		}
		
        [Test]
        public async Task CanNoThanksCancel()
        {
	        (await Sut.ClickOnElement(Sut.NoThanksSmartActivationLink)).CurrentPageAs<AccountSelectionPage>();
        }

		[Test]
        public async Task CanGoToStep2()
        {
	        Sut.InformationCollectionAuthorizedCheckbox.IsChecked = false;
	        Sut = (await Sut.ClickOnElement(Sut.EnableSmartServicesButton)).CurrentPageAs<Step1EnableSmartFeaturesPage>();
			Assert.IsTrue(Sut.InformationCollectionAuthorizedValidation.TextContent.Contains("You need to check the box above before you can continue"));
			Assert.IsTrue(Sut.InformationCollectionAuthorizedCheckbox.Attributes["class"].Value.Contains("error"));
			Sut.InformationCollectionAuthorizedCheckbox.IsChecked = true;
			(await Sut.ClickOnElement(Sut.EnableSmartServicesButton)).CurrentPageAs<Step2SelectPlanPage>();
		}

        [Test]
        public async Task CanGoToPrivacyNotice()
        {
	        Sut.InformationCollectionAuthorizedCheckbox.IsChecked = true;
	        (await Sut.ClickOnElement(Sut.SmartActivationPrivacyNoticeLink)).CurrentPageAs<TermsInfoPage>();
        }
	}
}
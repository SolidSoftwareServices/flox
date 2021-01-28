using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.Settings;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.PromotionEntry.Steps
{
    [TestFixture]
    internal class PromotionEntryTests
    {
	    private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.PromotionEntry.Steps.PromotionEntry, ResidentialPortalFlowType> NewScreenTestConfigurator(
		    bool anonymousUser = false)
	    {
		    return NewScreenTestConfigurator(anonymousUser
			    ? new DomainFacade()
			    : new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
	    }

	    private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.PromotionEntry.Steps.PromotionEntry, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade)
	    {
		    if (domainFacade == null) domainFacade = new DomainFacade();

		    return new FlowScreenTestConfigurator<WebApp.Flows.AppFlows.PromotionEntry.Steps.PromotionEntry, ResidentialPortalFlowType>(
				    domainFacade.ModelsBuilder)
			    .WithStub(domainFacade.SessionProvider)
			    .WithStub(domainFacade.QueryResolver)
			    .WithStub(domainFacade.CommandDispatcher);
	    }

	    [Test]
	    public void FlowTypeIsCorrect()
	    {
		    Assert.AreEqual(ResidentialPortalFlowType.PromotionEntry, NewScreenTestConfigurator().Adapter.GetFlowType());
	    }

	    private static AppUserConfigurator ConfigureDomain()
	    {
		    var cfg = new AppUserConfigurator(new DomainFacade())
			    .SetValidSession();
		    cfg.AddElectricityAccount(configureDefaultDevice: false);

		    cfg.Execute();
		    return cfg;
	    }

        [Test]
        public async Task Can_Get_CorrectConfig()
        {
            var cfg = ConfigureDomain();
            var fixture = new Fixture().CustomizeFrameworkBuilders();

            var promotionHeading = fixture.Create<string>();
            var promotionDescription1 = fixture.Create<string>();
            var promotionDescription2 = fixture.Create<string>();
            var promotionLinkText = fixture.Create<string>();
            var promotionLinkUrl = fixture.Create<string>();
            var promotionPageTitle = fixture.Create<string>();
            var promotionDescription3 = fixture.Create<string>();
            var promotionDescription4 = fixture.Create<string>();
            var promotionTermsConditionsLinkText = fixture.Create<string>();
            var promotionTermsConditionsLinkUrl = fixture.Create<string>();
			var imageDesktop = fixture.Create<string>();
            var imageMobile = fixture.Create<string>();
            var imageHeader = fixture.Create<string>();

			NewScreenTestConfigurator(cfg.DomainFacade)
                .WithMockConfiguration<IUiAppSettings>(c =>
                {
	                c.Setup(s => s.PromotionHeading).Returns(promotionHeading);
	                c.Setup(s => s.PromotionDescription1).Returns(promotionDescription1);
	                c.Setup(s => s.PromotionDescription2).Returns(promotionDescription2);
	                c.Setup(s => s.PromotionLinkText).Returns(promotionLinkText);
	                c.Setup(s => s.PromotionLinkURL).Returns(promotionLinkUrl);
	                c.Setup(s => s.PromotionPageTitle).Returns(promotionPageTitle);
	                c.Setup(s => s.PromotionDescription3).Returns(promotionDescription3);
	                c.Setup(s => s.PromotionDescription4).Returns(promotionDescription4);
	                c.Setup(s => s.PromotionTermsConditionsLinkText).Returns(promotionTermsConditionsLinkText);
	                c.Setup(s => s.PromotionTermsConditionsLinkURL).Returns(promotionTermsConditionsLinkUrl);
	                c.Setup(s => s.PromotionImageDesktop).Returns(imageDesktop);
	                c.Setup(s => s.PromotionImageMobile).Returns(imageMobile);
	                c.Setup(s => s.PromotionImageHeader).Returns(imageHeader);

				})
                .NewTestCreateStepDataRunner()
                .WhenCreated()
                .ThenTheStepDataIs<WebApp.Flows.AppFlows.PromotionEntry.Steps.PromotionEntry.ScreenModel>(actual =>
                {
					Assert.AreEqual(promotionHeading, actual.PromotionHeading);
					Assert.AreEqual(promotionDescription1, actual.PromotionDescription1);
					Assert.AreEqual(promotionDescription2, actual.PromotionDescription2);
					Assert.AreEqual(promotionLinkText, actual.PromotionLinkText);
					Assert.AreEqual(promotionLinkUrl, actual.PromotionLinkURL);
					Assert.AreEqual(promotionPageTitle, actual.PromotionPageTitle);
					Assert.AreEqual(promotionDescription3, actual.PromotionDescription3);
					Assert.AreEqual(promotionDescription4, actual.PromotionDescription4);
					Assert.AreEqual(promotionTermsConditionsLinkText, actual.PromotionTermsConditionsLinkText);
					Assert.AreEqual(promotionTermsConditionsLinkUrl, actual.PromotionTermsConditionsLinkURL);
					Assert.AreEqual(imageDesktop, actual.PromotionImageDesktop);
					Assert.AreEqual(imageMobile, actual.PromotionImageMobile);
					Assert.AreEqual(imageHeader, actual.PromotionImageHeader);
				});
        }
    }
}

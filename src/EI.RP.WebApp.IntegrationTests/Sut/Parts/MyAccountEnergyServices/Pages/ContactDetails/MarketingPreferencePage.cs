using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.ContactDetails
{
	internal class MarketingPreferencePage : MyAccountEnergyServicesPage
    {
	    public MarketingPreferencePage(ResidentialPortalApp app) : base(app)
	    {
	    }
	    protected override bool IsInPage()
	    {
		    return base.IsInPage() && Page != null;
	    }

	    public IHtmlElement Page => Document.QuerySelector("[data-pageid='marketing-preferences']") as IHtmlElement;
	}
}

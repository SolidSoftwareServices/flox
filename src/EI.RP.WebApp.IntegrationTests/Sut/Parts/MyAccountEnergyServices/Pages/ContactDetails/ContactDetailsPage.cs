using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.ContactDetails
{
	internal class ContactDetailsPage : MyAccountEnergyServicesPage

	{
		public ContactDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var result = base.IsInPage() && Page != null;
			return result;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-pageid='contact-user-detail']") as IHtmlElement;
	}
}

using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Graph.Pages
{
	internal class DotGraphPage : SutPage<ResidentialPortalApp>
	{
		public DotGraphPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return GraphDefinition != null;
		}

		public IHtmlDivElement GraphDefinition => (IHtmlDivElement) Document.GetElementById("svg_target");
	}
}
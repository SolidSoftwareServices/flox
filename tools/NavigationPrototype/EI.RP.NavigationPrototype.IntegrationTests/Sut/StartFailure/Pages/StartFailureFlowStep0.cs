using System;
using AngleSharp.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.StartFailure.Pages
{
	class StartFailureFlowStep0 : SutPage<PrototypeApp>
	{
		public StartFailureFlowStep0(PrototypeApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return "Redirected correctly to this error view" ==
			       Document.Body.QuerySelector("#StartFailureFlowPage > div > div > h2")?.TextContent;
		}
	}
}
using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages
{
	class BlueFlowCompleted : SutPage<PrototypeApp>
	{
		private IHtmlElement _node;


		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div > div > h2")?.TextContent == "BlueFlow::Process Completed";
		}

		public BlueFlowCompleted(PrototypeApp app) : base(app)
		{
		}
		public BlueFlowCompleted(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}
	}
}
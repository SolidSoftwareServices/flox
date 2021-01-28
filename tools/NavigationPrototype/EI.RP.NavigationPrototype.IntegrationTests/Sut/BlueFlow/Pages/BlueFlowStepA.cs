using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages
{
	class BlueFlowStepA : SutPage<PrototypeApp>
	{
		private IHtmlElement _node;
		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "Process BlueFlow::StepAScreen";
		}

		public BlueFlowStepA(PrototypeApp app) : base(app)
		{
		}
		public BlueFlowStepA(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}

		public IEnumerable<string> Errors()
		{
			return _node.QuerySelectorAll("div:nth-child(3) > div > div > ul > li").Select(x => x.TextContent).Where(x => x.Trim().Any());
		}
		public IHtmlInputElement Input => _node.QuerySelector("#StepAValue1") as IHtmlInputElement;
		public IHtmlInputElement AntiforgeryToken => _node.QuerySelector("input[type=hidden][name=__RequestVerificationToken]") as IHtmlInputElement;

		public BlueFlowStepA InputValues(string input)
		{
			Input.Value = input;
			return this;
		}
		public async Task<ISutApp> Next()
		{
			return await ClickOnElementByText("Next");
		}

		public async Task<ISutApp> Previous()
		{
			return await ClickOnElementByText("Previous");
		}
	}
}
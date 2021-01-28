using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages
{
	class GreenFlowStepA : SutPage<PrototypeApp>
	{
		private IHtmlElement _node;


		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "Process GreenFlow::StepAScreen";
		}

		public GreenFlowStepA(PrototypeApp app) : base(app)
		{
		}

		public IEnumerable<string> Errors()
		{
			return _node.QuerySelectorAll("div:nth-child(3) > div > div > ul > li").Select(x => x.TextContent).Where(x => x.Trim().Any());
		}
		public IHtmlInputElement Input => _node.QuerySelector("#StepAValue1") as IHtmlInputElement;
		public IHtmlInputElement AntiforgeryToken => _node.QuerySelector("input[type=hidden][name=__RequestVerificationToken]") as IHtmlInputElement;

		public GreenFlowStepA InputValues(string input)
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
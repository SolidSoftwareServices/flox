using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages
{
	class BlueFlowStepC : SutPage<PrototypeApp>
	{
		private IHtmlElement _node;

		

		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "Process BlueFlow::StepCScreen";
		}
		public BlueFlowStepC(PrototypeApp app) : base(app)
		{
		}
		public BlueFlowStepC(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}
		public string InitialScreenValue =>
			_node.QuerySelector(" div:nth-child(2) > div > label").TextContent.Split(':')[1];

		public int ViewDataValue => int.Parse(_node.QuerySelector("div:nth-child(3) > div:nth-child(5) > label").TextContent.Split(':')[1]);
		public string StepAValue => _node.QuerySelector("div:nth-child(3) > div > label").TextContent.Split(':')[1];
		public string StepBValue => _node.QuerySelector("div:nth-child(4) > div > label").TextContent.Split(':')[1];

		public async Task<ISutApp> Previous()
		{
			return await ClickOnElementByText("Previous");
		}
		public async Task<ISutApp> Complete()
		{
			return await ClickOnElementByText("Complete");
		}
	}
}
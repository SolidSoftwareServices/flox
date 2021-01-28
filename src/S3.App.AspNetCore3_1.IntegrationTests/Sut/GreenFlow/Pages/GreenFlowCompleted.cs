using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Http;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages
{
	class GreenFlowCompleted : SutPage<PrototypeApp>
	{

		private IHtmlElement _node;

		public GreenFlowCompleted(PrototypeApp app) : base(app)
		{
		}
		public GreenFlowCompleted(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}

		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "GreenFlow::Process Completed";
		}

	

		public IHtmlInputElement BlueFlowValue=> _node.GetElementById("BlueFlowInitialScreenValue") as IHtmlInputElement;
		public IHtmlInputElement BlueFlowEventHandled => _node.GetElementById("BlueFlowEventHandled") as IHtmlInputElement;

	}
}
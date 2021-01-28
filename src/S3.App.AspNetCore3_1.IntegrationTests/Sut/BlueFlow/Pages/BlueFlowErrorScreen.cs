using System;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages
{
	class BlueFlowErrorScreen : SutPage<PrototypeApp>
	{
		private IHtmlElement _node;
		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "BlueFlow::ErrorScreen";
		}

		public BlueFlowErrorScreen(PrototypeApp app) : this(app, null)
		{
		}
		public BlueFlowErrorScreen(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}


		public string Error => _node.QuerySelector("#ErrorScreen > div:nth-child(2) > div").TextContent.Trim().Substring("Error".Length+1).Trim();
		public string Step => _node.QuerySelector("#ErrorScreen > div:nth-child(3) > div").TextContent.Trim().Substring("OnStep".Length + 1).Trim();
		public string OnLifecycleEvent => _node.QuerySelector("#ErrorScreen > div:nth-child(4) > div").TextContent.Trim().Substring("OnLifecycleEvent".Length + 1).Trim();

	}
}
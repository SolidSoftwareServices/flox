using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Html;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages
{
	class GreenFlowStep0: SutPage<PrototypeApp>
	{
		private IHtmlElement _node;


		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent ==  "GreenFlow::Step 0";
		}

		public GreenFlowStep0(PrototypeApp app) : base(app)
		{
		}
		public GreenFlowStep0(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}


		public IHtmlInputElement Input => Document.QuerySelector("#StepValue1") as IHtmlInputElement;

		public string FieldValidatorValue =>
			Document.QuerySelector("#GreenFlow- > div:nth-child(2) > div > span").Text();


		public IEnumerable<string> Errors()
		{
			return _node.QuerySelectorAll("#ValidationSummary >  div > ul > li").Select(x=>x.TextContent).Where(x=>x.Trim().Any());
		}

		public GreenFlowStep0 InputValues(string input)
		{
			Input.Value = input;
			return this;
		}
		public async Task<ISutApp> Next()
		{
			return await ClickOnElementByText("Next");
		}


		public async Task<ISutApp> Reset()
		{
			return await ClickOnElementByText("Reset");
		}
	}
}
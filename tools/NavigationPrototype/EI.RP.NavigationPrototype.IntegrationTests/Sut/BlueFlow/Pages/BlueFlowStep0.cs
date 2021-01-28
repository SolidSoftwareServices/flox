using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Html;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages
{
	class BlueFlowStep0: SutPage<PrototypeApp>
	{
		private IHtmlElement _node;
		protected override bool IsInPage()
		{
			_node= _node??(IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "BlueFlow::Step 0";
		}

		public BlueFlowStep0(PrototypeApp app) : this(app,null)
		{
		}
		public BlueFlowStep0(PrototypeApp app,IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node!=null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}


		public IHtmlInputElement Input => _node.QuerySelector("#StepValue1") as IHtmlInputElement;
		public IHtmlInputElement SampleInput => _node.QuerySelector("#StringValue") as IHtmlInputElement;

		public string FieldValidatorValue =>
			_node.QuerySelector("#BlueFlow- > div:nth-child(2) > div > span").Text();


		public IEnumerable<string> Errors()
		{
			return _node.QuerySelectorAll("#ValidationSummary > div > ul > li").Select(x=>x.TextContent).Where(x=>x.Trim().Any());
		}


		public async Task<ISutApp> ClickOnOpenSibling()
		{
			return await App.ClickOnElement(Document.GetElementByText("open GreenFlow from sibling"));
		}

		public BlueFlowStep0 InputValues(string input, string sampleInput)
		{
			Input.Value = input;
			SampleInput.Value = sampleInput;
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

		public async Task<ISutApp> ClickFailInitialisation()
		{
			return await ClickOnElementById("lnkFailInitialization");
		}

		public async Task<ISutApp> ClickFailCreatingScreen()
		{
			return await ClickOnElementById("lnkFailCreatingStep");
		}
	}
}
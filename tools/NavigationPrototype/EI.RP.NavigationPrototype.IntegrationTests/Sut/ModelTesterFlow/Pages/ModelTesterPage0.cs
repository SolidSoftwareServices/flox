using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AutoFixture;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Html;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ModelTesterFlow.Pages
{
	class ModelTesterPage0: SutPage<PrototypeApp>
	{


		protected override bool IsInPage()
		{
			var node=Document.Body.QuerySelector("#ModelTesterFlowPage > h2");
			return this.IsInModelTesterFlowPageFlow() && node != null && node.TextContent == "Test inputs here:";
		}

		public ModelTesterPage0(PrototypeApp app) : base(app)
		{
		}

		public static IDictionary<string, string> GenerateRandomValues()
		{
			var fixture = new Fixture();
			IDictionary<string, string> generatedValues = new Dictionary<string, string>
			{
				{nameof(ModelTesterPage0.SampleInput), fixture.Create<string>()},
				{nameof(ModelTesterPage0.RequiredIf), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Root1), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Root2), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested1_Level1), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested1_Level2), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested1_Level3), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested2_Level1), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested2_Level2), fixture.Create<string>()},
				{nameof(ModelTesterPage0.Nested2_Level3), fixture.Create<string>()},
			};
			return generatedValues;
		}
		public IHtmlInputElement SampleInput => Document.QuerySelector("#StringValue") as IHtmlInputElement;
		public IHtmlInputElement RequiredIf => Document.QuerySelector("#RequiredOnlyIfNestedValue1DoesNotHaveAValue") as IHtmlInputElement;
		public IHtmlInputElement Root1 => Document.QuerySelector("#StepValue1") as IHtmlInputElement;
		public IHtmlInputElement Root2 => Document.QuerySelector("#StepValue2") as IHtmlInputElement;
		public IHtmlInputElement Nested1_Level1 => Document.QuerySelector("#Nested1_NestedValue1") as IHtmlInputElement;
		public IHtmlInputElement Nested1_Level2 => Document.QuerySelector("#Nested1_Nested_NestedValue1") as IHtmlInputElement;
		public IHtmlInputElement Nested1_Level3 => Document.QuerySelector("#Nested1_Nested_Nested_NestedValue1") as IHtmlInputElement;
		public IHtmlInputElement Nested2_Level1 => Document.QuerySelector("#Nested2_NestedValue1") as IHtmlInputElement;
		public IHtmlInputElement Nested2_Level2 => Document.QuerySelector("#Nested2_Nested_NestedValue1") as IHtmlInputElement;
		public IHtmlInputElement Nested2_Level3 => Document.QuerySelector("#Nested2_Nested_Nested_NestedValue1") as IHtmlInputElement;

		public IEnumerable<string> Errors()
		{
			return Document.QuerySelectorAll("#validationSummary > div > ul > li").Select(x=>x.TextContent).Where(x=>x.Trim().Any());
		}


		public async Task<PrototypeApp> ClickOnOpenSibling(SampleAppFlowType flowType)
		{
			return (PrototypeApp)await App.ClickOnElement(Document.GetElementByText($"sibling {flowType}"));
		}

		public ModelTesterPage0 SetValues(params (string, string)[] values)
		{
			foreach (var (item1, item2) in values)
			{
				switch (item1)
				{
					case nameof(SampleInput):
						SampleInput.Value = item2;
						break;
					case nameof(RequiredIf):
						RequiredIf.Value = item2;
						break;
					case nameof(Root1):
						Root1.Value = item2;
						break;
					case nameof(Root2):
						Root2.Value = item2;
						break;
					case nameof(Nested1_Level1):
						Nested1_Level1.Value = item2;
						break;
					case nameof(Nested1_Level2):
						Nested1_Level2.Value = item2;
						break;
					case nameof(Nested1_Level3):
						Nested1_Level3.Value = item2;
						break;
					case nameof(Nested2_Level1):
						Nested2_Level1.Value = item2;
						break;
					case nameof(Nested2_Level2):
						Nested2_Level2.Value = item2;
						break;
					case nameof(Nested2_Level3):
						Nested2_Level3.Value = item2;
						break;
					default: throw new InvalidOperationException($"{item1} not found");
				}
			}

			return this;
		}

		public void AssertValues(params (string, string)[] values)
		{
			foreach (var (item1, item2) in values)
			{
				switch (item1)
				{
					case nameof(SampleInput):
						Assert.IsTrue(SampleInput.Value == item2);
						break;
					case nameof(RequiredIf):
						Assert.IsTrue(RequiredIf.Value == item2);
						break;
					case nameof(Root1):
						Assert.IsTrue(Root1.Value == item2);
						break;
					case nameof(Root2):
						Assert.IsTrue(Root2.Value == item2);
						break;
					case nameof(Nested1_Level1):
						Assert.IsTrue(Nested1_Level1.Value == item2);
						break;
					case nameof(Nested1_Level2):
						Assert.IsTrue(Nested1_Level2.Value == item2);
						break;
					case nameof(Nested1_Level3):
						Assert.IsTrue(Nested1_Level3.Value == item2);
						break;
					case nameof(Nested2_Level1):
						Assert.IsTrue(Nested2_Level1.Value== item2);
						break;
					case nameof(Nested2_Level2):
						Assert.IsTrue(Nested2_Level2.Value == item2);
						break;
					case nameof(Nested2_Level3):
						Assert.IsTrue(Nested2_Level3.Value == item2);
						break;
					default: throw new InvalidOperationException($"{item1} not found");
				}
			}
		}

		public async Task<ISutApp> SubmitInvalidEvent()
		{
			return await ClickOnElementByText("An Invalid Event");
		}
	}
}
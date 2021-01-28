using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ModelTesterFlow.Pages
{
	class ModelTesterCompleted : SutPage<PrototypeApp>
	{
		public ModelTesterCompleted(PrototypeApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var textContent = Document.Body.QuerySelector("#ModelTesterFlowPage > div:nth-child(1) > div > h2")?.TextContent;
			return this.IsInModelTesterFlowPageFlow() && "Data" == textContent;
		}

		public string NotUsed=>((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(2) > div > span")).Text().Trim() ;

		public string Root1 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(3) > div > span")).Text().Trim();
		public string Root2 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(4) > div > span")).Text().Trim();
		public string Nested1_Level1 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(5) > div > span")).Text().Trim();
		public string Nested1_Level2 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(6) > div > span")).Text().Trim();
		public string Nested1_Level3 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(7) > div > span")).Text().Trim();
		public string Nested2_Level1 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(8) > div > span")).Text().Trim();
		public string Nested2_Level2 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(9) > div > span")).Text().Trim();
		public string Nested2_Level3 => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(10) > div > span")).Text().Trim();
		public string SampleInput => ((IHtmlSpanElement)Document.QuerySelector("#ModelTesterFlowPage > div:nth-child(11) > div > span")).Text().Trim();
		public void AssertValues((string, string)[] values)
		{
			Assert.IsTrue(Guid.TryParse(NotUsed,out Guid r));
			foreach (var (item1, item2) in values)
			{
				switch (item1)
				{
					
					case nameof(Root1):
						Assert.IsTrue(Root1 == item2);
						break;
					case nameof(Root2):
						Assert.IsTrue(Root2 == item2);
						break;
					case nameof(Nested1_Level1):
						Assert.IsTrue(Nested1_Level1 == item2);
						break;
					case nameof(Nested1_Level2):
						Assert.IsTrue(Nested1_Level2 == item2);
						break;
					case nameof(Nested1_Level3):
						Assert.IsTrue(Nested1_Level3 == item2);
						break;
					case nameof(Nested2_Level1):
						Assert.IsTrue(Nested2_Level1 == item2);
						break;
					case nameof(Nested2_Level2):
						Assert.IsTrue(Nested2_Level2 == item2);
						break;
					case nameof(Nested2_Level3):
						Assert.IsTrue(Nested2_Level3 == item2);
						break;
					case nameof(SampleInput):
						Assert.IsTrue(SampleInput == item2);
						break;
					case "RequiredIf": break;
					default: throw new InvalidOperationException($"{item1} not found");
				}
			}
		}
	}
}
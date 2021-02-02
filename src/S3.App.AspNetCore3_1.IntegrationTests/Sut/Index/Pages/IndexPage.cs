using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.Flows.AppFlows;
using S3.UI.TestServices.Html;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages
{
	class IndexPage: SutPage<PrototypeApp>
	{
		
		protected override bool IsInPage()
		{
			var node=Document.Body.QuerySelector("body > div > b:nth-child(1)");
			return node != null && node.TextContent == "Index Page";
		}

		public IndexPage(PrototypeApp app) : base(app)
		{
		}

		public async Task<PrototypeApp> SelectBlueFlow(string queryString=null)
		{
			return await SelectFlow(SampleAppFlowType.BlueFlow.ToString(),queryString);
		}
		public async Task<PrototypeApp> SelectGreenFlow(string queryString = null)
		{
			return await SelectFlow(SampleAppFlowType.GreenFlow.ToString(), queryString);
		}

		public async Task<PrototypeApp> SelectModelTesterFlow()
		{
			return await SelectFlow(SampleAppFlowType.ModelTesterFlow.ToString(),null);
		}



		public async Task<PrototypeApp> SelectStartFailureFlow()
		{
			return await SelectFlow(SampleAppFlowType.StartFailure.ToString(), null);
		}

		private async Task<PrototypeApp> SelectFlow(string byText,string queryString)
		{
			var element = (IHtmlAnchorElement) Document.GetElementByText(byText);
			if (!string.IsNullOrEmpty(queryString))
			{
				element.Href += $"?{queryString}";
			}

			return (PrototypeApp) await App.ClickOnElement(element);
		}


		public async Task<PrototypeApp> SelectContainersFlow()
		{
			return await SelectFlow(SampleAppFlowType.ContainersFlow.ToString(), null);
		}
		public async Task<PrototypeApp> SelectContainersFlow2()
		{
			return await SelectFlow(SampleAppFlowType.ContainersFlow2.ToString(), null);
		}
	}
}
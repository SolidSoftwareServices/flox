using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.Flows.AppFlows;
using S3.UI.TestServices.Html;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages
{
	abstract class ContainerPage : SutPage<PrototypeApp>, IContainerPage
	{
		protected IHtmlElement Root { get; set; }

		protected ContainerPage(PrototypeApp app) : base(app)
		{
		}

		public async Task<PrototypeApp> SelectBlueFlow()
		{
			var text = "BlueFlow";
			return await SelectContained(text);
		}
		public async Task<PrototypeApp> SelectGreenFlow()
		{
			var text ="GreenFlow";
			return await SelectContained(text);
		}
		private async Task<PrototypeApp> SelectContained(string text)
		{
			return (PrototypeApp)await App.ClickOnElement(Root.GetElementByText(text,false));
		}

		public async Task<PrototypeApp> SelectContainerFlow()
		{
			return await SelectContained("ContainersFlow");
		}
		protected override bool IsInPage()
		{
			Root = Root??(IHtmlElement)Document.QuerySelector("body > div");
			return Root?.Id==FlowPageId &&  Root?.QuerySelector($"#{FlowPageId} > div:nth-child(1) > div > h2")?.TextContent ==HeaderText;

		}

		public abstract string FlowPageId { get; }

		public abstract string HeaderText { get; }


		public TPage GetCurrentContained<TPage>() where TPage:ISutPage
		{
			//contained-part
			var containerElement=Root.QuerySelector("#contained-part");
			var containedPart = (TPage)Activator.CreateInstance(typeof(TPage), new object[] { App, containerElement });
		
			return containedPart;
		
		}
	}
}
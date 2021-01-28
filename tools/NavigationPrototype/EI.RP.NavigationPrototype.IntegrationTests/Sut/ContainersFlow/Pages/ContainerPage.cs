using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.UI.TestServices.Html;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages
{
	abstract class ContainerPage : SutPage<PrototypeApp>, IContainerPage
	{
		protected IHtmlElement Root { get; set; }

		protected ContainerPage(PrototypeApp app) : base(app)
		{
		}

		public async Task<PrototypeApp> SelectBlueFlow()
		{
			var text = SampleAppFlowType.BlueFlow.ToString();
			return await SelectContained(text);
		}
		public async Task<PrototypeApp> SelectGreenFlow()
		{
			var text = SampleAppFlowType.GreenFlow.ToString();
			return await SelectContained(text);
		}
		private async Task<PrototypeApp> SelectContained(string text)
		{
			return (PrototypeApp)await App.ClickOnElement(Root.GetElementByText(text));
		}

		public async Task<PrototypeApp> SelectContainerFlow()
		{
			return await SelectContained(SampleAppFlowType.ContainersFlow.ToString());
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
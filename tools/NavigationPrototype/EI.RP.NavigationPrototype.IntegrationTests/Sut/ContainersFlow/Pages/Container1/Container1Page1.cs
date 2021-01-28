using System;
using System.Linq;
using AngleSharp.Html.Dom;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1
{
	internal class Container1Page1 : ContainerPage
	{
		public Container1Page1(PrototypeApp app) : base(app)
		{
		}
		public Container1Page1(PrototypeApp app, IHtmlElement root) : base(app)
		{
			Root = root?.Children.FirstOrDefault() as IHtmlElement;
			if (Root != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}
		public override string HeaderText { get; } = "Containers sample Number2Step";
		public override string FlowPageId { get; } = "ContainersFlowPage";
	}
}
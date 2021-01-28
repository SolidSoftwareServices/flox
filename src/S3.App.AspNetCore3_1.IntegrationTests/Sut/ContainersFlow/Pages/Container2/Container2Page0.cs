using System;
using System.Linq;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container2
{
	class Container2Page0 : ContainerPage
	{
		public Container2Page0(PrototypeApp app) : base(app)
		{
		}
		public Container2Page0(PrototypeApp app, IHtmlElement root) : base(app)
		{
			Root = root?.Children.FirstOrDefault() as IHtmlElement;
			if (Root != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}
		public override string HeaderText { get; } = "Containers 2 sample Number1Step";
		public override string FlowPageId { get; } = "Containers2FlowPage";
	}
}
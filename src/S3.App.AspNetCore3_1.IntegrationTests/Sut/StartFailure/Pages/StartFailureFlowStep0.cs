using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.StartFailure.Pages
{
	class StartFailureFlowStep0 : SutPage<PrototypeApp>
	{
		public StartFailureFlowStep0(PrototypeApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return "Redirected correctly to this error view" ==
			       Document.Body.QuerySelector("#StartFailureFlowPage > div > div > h2")?.TextContent;
		}
	}
}
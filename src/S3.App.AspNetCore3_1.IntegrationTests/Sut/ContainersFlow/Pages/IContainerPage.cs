using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages
{
	interface IContainerPage: ISutPage
	{
		TPage GetCurrentContained<TPage>() where TPage : ISutPage;
	}
}
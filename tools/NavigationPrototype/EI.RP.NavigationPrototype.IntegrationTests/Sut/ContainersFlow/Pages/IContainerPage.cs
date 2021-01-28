using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages
{
	interface IContainerPage: ISutPage
	{
		TPage GetCurrentContained<TPage>() where TPage : ISutPage;
	}
}
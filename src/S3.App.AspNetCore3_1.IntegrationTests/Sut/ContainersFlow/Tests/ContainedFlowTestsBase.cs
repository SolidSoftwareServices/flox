using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests
{
	internal abstract class ContainedFlowTestsBase<TRootContainerPage> : WebAppPageTests<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{

		public virtual TStep AsStep<TStep>() where TStep : ISutPage
		{
			return ResolveImmediateContainer().GetCurrentContained<TStep>();
		}

		protected abstract IContainerPage ResolveImmediateContainer();
	}
}
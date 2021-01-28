using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow
{
	internal abstract class ContainedBlueFlowTestsBase<TRootContainerPage> : WebAppPageTests<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{
		public virtual BlueFlowStep0 AsStep0()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStep0>();

		}

		public virtual BlueFlowStepA AsStepA()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepA>();
		}

		public virtual BlueFlowStepB AsStepB()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepB>();
		}

		public virtual BlueFlowStepC AsStepC()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepC>();
		}

		protected abstract IContainerPage ResolveImmediateContainer();
	}
}
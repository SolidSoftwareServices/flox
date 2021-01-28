using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow
{
	internal abstract class ContainedGreenFlowTestsBase<TRootContainerPage> : WebAppPageTests<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{
		public virtual GreenFlowStep0 AsStep0()
		{
			return ResolveImmediateContainer().GetCurrentContained<GreenFlowStep0>();

		}

		public virtual GreenFlowStepA AsStepA()
		{
			return ResolveImmediateContainer().GetCurrentContained<GreenFlowStepA>();
		}

		public virtual GreenFlowStepB AsStepB()
		{
			return ResolveImmediateContainer().GetCurrentContained<GreenFlowStepB>();
		}

		public virtual GreenFlowStepC AsStepC()
		{
			return ResolveImmediateContainer().GetCurrentContained<GreenFlowStepC>();
		}

		protected abstract IContainerPage ResolveImmediateContainer();
	}
}
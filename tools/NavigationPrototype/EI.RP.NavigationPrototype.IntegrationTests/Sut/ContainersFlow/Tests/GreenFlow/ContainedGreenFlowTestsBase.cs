using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow
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
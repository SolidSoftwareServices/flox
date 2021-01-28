using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow
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
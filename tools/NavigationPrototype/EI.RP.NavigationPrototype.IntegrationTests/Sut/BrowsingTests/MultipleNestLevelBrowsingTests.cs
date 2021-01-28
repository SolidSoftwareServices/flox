using System;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.BrowsingTests
{
	[TestFixture]
	class MultipleNestLevelBrowsingTests : BrowsingTests
	{
		protected override async Task TestScenarioArrangement()
		{
			var container2FlowPage = await App.ToContainer2Flow();
			await container2FlowPage.SelectContainerFlow();
			container2FlowPage = App.CurrentPageAs<Container2Page0>();

			var containersPage = container2FlowPage.GetCurrentContained<Container1Page0>();
			await containersPage.SelectBlueFlow();
			Assert.IsNotNull(AsStep0());
		}
		private Container1Page0 ResolveContainer()
		{
			return App.CurrentPageAs<Container2Page0>().GetCurrentContained<Container1Page0>();
		}


		public override BlueFlowStep0 AsStep0()
		{
			return ResolveContainer().GetCurrentContained<BlueFlowStep0>();
		}

		
		public override BlueFlowStepA AsStepA()
		{
			return ResolveContainer().GetCurrentContained<BlueFlowStepA>();
		}

		public override BlueFlowStepB AsStepB()
		{
			return ResolveContainer().GetCurrentContained<BlueFlowStepB>();
		}

		public override BlueFlowStepC AsStepC()
		{
			return ResolveContainer().GetCurrentContained<BlueFlowStepC>();
		}
	}
}
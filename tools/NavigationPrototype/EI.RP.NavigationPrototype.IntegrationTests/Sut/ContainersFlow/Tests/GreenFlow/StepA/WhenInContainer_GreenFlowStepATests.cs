using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepA
{
	[TestFixture]
	internal abstract class WhenInContainer_GreenFlowStepATests<TRootContainerPage> : ContainedGreenFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{

		protected async Task<GreenFlowStepA> ResolveSut(GreenFlowStep0 step0)
		{
			
			await step0.InputValues("ggg")
				.ClickOnElementByText("Next");
			return AsStepA();
		}
		protected GreenFlowStepA PageSut { get; set; }


	
	}
}
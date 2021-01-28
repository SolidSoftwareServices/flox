using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.UI.TestServices.Sut;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepA
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
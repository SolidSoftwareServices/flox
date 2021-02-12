using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepA
{
	[TestFixture]
	internal abstract class WhenInContainer_GreenFlowStepATests<TRootContainerPage> : ContainedFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{

		protected async Task<GreenFlowStepA> ResolveSut(GreenFlowStep0 step0)
		{
			
			await step0.InputValues("ggg")
				.ClickOnElementByText("Next");
			return AsStep<GreenFlowStepA>();
		}
		protected GreenFlowStepA PageSut { get; set; }


	
	}
}
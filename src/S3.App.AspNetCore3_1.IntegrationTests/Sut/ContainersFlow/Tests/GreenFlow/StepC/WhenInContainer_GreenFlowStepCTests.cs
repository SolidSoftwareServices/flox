using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.Flows.AppFlows.GreenFlow.Steps;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
{
	[TestFixture]
	internal abstract class
		WhenInContainer_GreenFlowStepCTests<TRootContainerPage> : ContainedFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{


		protected async Task<GreenFlowStepC> ResolveSut(GreenFlowStep0 step0)
		{
			await step0.InputValues("aa").Next();
			return AsStep<GreenFlowStepC>();
		}

		protected GreenFlowStepC PageSut { get; set; }

		public virtual (string, string)[] InputQueryStringParameters { get; } =   {(nameof(FlowInitializer.StartScreenModel.SampleParameter), "1asdfasd")};


		private GreenFlowCompleted AsGreenFlowCompleted()
		{
			return ResolveImmediateContainer().GetCurrentContained<GreenFlowCompleted>();
		}


		public virtual BlueFlowStep0 AsBlueStep0()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStep0>();

		}

		public virtual BlueFlowStepA AsBlueStepA()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepA>();
		}

		public virtual BlueFlowStepB AsBlueStepB()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepB>();
		}

		public virtual BlueFlowStepC AsBlueStepC()
		{
			return ResolveImmediateContainer().GetCurrentContained<BlueFlowStepC>();
		}
		[Test,Ignore("not sure it should be supported")]
		public async Task InputParametersAreCorrect()
		{
			AssertInputParameters();
		}

		private void AssertInputParameters()
		{
			Assert.AreEqual(
				$"Flow start value:{string.Join(" - ", InputQueryStringParameters.Select(x => $"{x.Item2}"))}",
				AsStep<GreenFlowStepC>().LabelStartValue.TextContent);
		}


		[Test]
		public async Task CanStartBlueFlowAndCollectResult()
		{
			var expected = DateTime.UtcNow.Ticks.ToString();
			await AsStep<GreenFlowStepC>().StartBlueFlow();
			var s0 = AsBlueStep0();
			s0.InputValues("a", expected);
			await s0.Next();
			var sc = AsBlueStepC();
			await sc.Complete();
			var resultPage = AsGreenFlowCompleted();

			Assert.AreEqual(expected, resultPage.BlueFlowValue.Value);
			Assert.IsTrue(bool.Parse(resultPage.BlueFlowEventHandled.Value));
		}

	}
}
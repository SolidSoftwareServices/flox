using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
{
	[TestFixture]
	internal abstract class
		WhenInContainer_GreenFlowStepCTests<TRootContainerPage> : ContainedGreenFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{


		protected async Task<GreenFlowStepC> ResolveSut(GreenFlowStep0 step0)
		{
			await step0.InputValues("aa").Next();
			return AsStepC();
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
				AsStepC().LabelStartValue.TextContent);
		}


		[Test]
		public async Task CanStartBlueFlowAndCollectResult()
		{
			var expected = DateTime.UtcNow.Ticks.ToString();
			await AsStepC().StartBlueFlow();
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
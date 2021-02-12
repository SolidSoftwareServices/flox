using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq.AutoMock;
using NUnit.Framework;
using S3.TestServices;
using S3.UiFlows.Core.DataSources.Repositories;
using S3.UiFlows.Core.DataSources.Repositories.Adapters;
using S3.UiFlows.Core.DataSources.Stores;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.UnitTests.Data
{
	[TestFixture]
	class UiFlowContextDataRepositoryDecoratorTests : UnitTestFixture<UiFlowContextDataRepositoryDecoratorTests.TestContext, UiFlowContextDataRepositoryDecorator>
	{
		[Test]
		public void CanCreateNew()
		{
			var actual = Context.Sut.CreateNew();	
			Assert.AreEqual(ScreenName.PreStart,actual.CurrentScreenStep);
		}

		[Test]
		public void ItCreatesFlowHandlerCorrectly()
		{

			var expected = Context.Sut.CreateNew();
			Assert.IsNotNull(!string.IsNullOrWhiteSpace(expected.FlowHandler));
		}

		[Test]
		public async Task ItCreatesANewInstanceOnCreateSnapshot()
		{
			var expected = Context.Sut.CreateNew();
			var actual = await Context.Sut.CreateSnapshotOfContext(expected);
			Assert.AreNotSame(expected, actual);
				
		}

		[Test]
		public async Task CreateSnapshot_ItConnectsSnapshotWithPrevious()
		{
			var expected = Context.Sut.CreateNew();
			var actual = await Context.Sut.CreateSnapshotOfContext(expected);
			Assert.AreEqual(expected.NextFlowHandler,actual.FlowHandler);

		}

		[Test]
		public async Task ItChangesStatusHandlerOnSave()
		{
			var expected = Context.Sut.CreateNew();
			var original = expected.FlowHandler;
			var actual = await Context.Sut.CreateSnapshotOfContext(expected);
			
			Assert.IsFalse(string.IsNullOrWhiteSpace(actual.FlowHandler));
			Assert.AreNotEqual(original, actual.FlowHandler);
		}

		[Test]
		public async Task ItUpdatesContextOnSaveWhenCreatingSnapshot()
		{

			var original = Context.Sut.CreateNew();
			var changed1 = await Context.Sut.CreateSnapshotOfContext(original);
			var changed2 = await Context.Sut.CreateSnapshotOfContext(changed1);
			Assert.AreNotEqual(changed1.FlowHandler, changed2.FlowHandler);
			Assert.AreNotEqual(changed1.FlowHandler, original.FlowHandler);
			Assert.AreNotSame(changed1, original);
			Assert.AreNotSame(changed2, original);
		}

		[Test]
		public async Task CreateSnapshotOfContext_ItAttachesNewSnapshot()
		{

			var original = Context.Sut.CreateNew();
			var changed1 = await Context.Sut.CreateSnapshotOfContext(original);
			
			Assert.AreSame(changed1,await Context.Sut.Get(changed1.FlowHandler));
		}

		

		[Test]
		public async Task CanLoadByFlowHandler()
		{
			
			var original = Context.Sut.CreateNew();
			var changed1=await Context.Sut.CreateSnapshotOfContext(original);
			var changed2 = await Context.Sut.CreateSnapshotOfContext(changed1);
			
			var actual=await Context.Sut.Get(changed1.FlowHandler);
			Assert.AreSame(changed1.FlowHandler,actual.FlowHandler);

			actual = await Context.Sut.Get(changed2.FlowHandler);
			Assert.AreSame(changed2.FlowHandler, actual.FlowHandler);
			actual = await Context.Sut.Get(original.FlowHandler);
			Assert.AreSame(original.FlowHandler, actual.FlowHandler);
		}

		[Test]
		public async Task ItUpdatesStepHandlersOnSave()
		{
			var flowA = Context.Sut.CreateNew();
			
			var stepDataA = flowA.AddStepData("stepDataA");
			var stepDataB = flowA.AddStepData("stepDataB");

			var changed=await Context.Sut.Get(flowA.FlowHandler);

			Assert.AreEqual(changed.FlowHandler,changed.ViewModels[stepDataA.FlowScreenName].FlowHandler);
			Assert.AreEqual(changed.FlowHandler, changed.ViewModels[stepDataB.FlowScreenName].FlowHandler);
		}



		[Test]
		public async Task ItDoesNotUpdateOriginalStepHandlersOnSave()
		{
			var flowA = Context.Sut.CreateNew();
			var stepDataA = flowA.AddStepData("stepDataA");
			var stepDataB = flowA.AddStepData("stepDataB");


			Assert.AreEqual(flowA.FlowHandler, flowA.ViewModels[stepDataA.FlowScreenName].FlowHandler);
			Assert.AreEqual(flowA.FlowHandler, flowA.ViewModels[stepDataB.FlowScreenName].FlowHandler);
		}

		[Test]
		public async Task ItUpdatesNextFlowSnapshotOnTransition()
		{
			var ctxA = Context.Sut.CreateNew();

			var stepDataA = ctxA.AddStepData("stepDataA",true);
			var originalCtxA = ctxA;
			Assert.IsNull(originalCtxA.NextFlowHandler);

			ctxA = await Context.Sut.CreateSnapshotOfContext(ctxA);
			Assert.AreNotSame(ctxA, originalCtxA);
			Assert.IsNull(ctxA.NextFlowHandler);

			var actualCtxA = await Context.Sut.Get(originalCtxA.FlowHandler);
			var ctxAFlowHandler = ctxA.FlowHandler;
			Assert.AreEqual(actualCtxA.NextFlowHandler,ctxAFlowHandler);
			ctxA = await Context.Sut.CreateSnapshotOfContext(ctxA);
			actualCtxA = await Context.Sut.Get(originalCtxA.FlowHandler);
			Assert.AreEqual(actualCtxA.NextFlowHandler, ctxAFlowHandler);
			Assert.AreNotEqual(ctxA.FlowHandler,actualCtxA.NextFlowHandler);
		}

		[Test]
		public async Task ItUpdatesContainersOnSaveSnapshot()
		{
			var expectedContainerA = Context.Sut.CreateNew();

			var stepDataA = expectedContainerA.AddStepData("stepDataA", true);
			var originalContainerA = expectedContainerA;
			expectedContainerA =await Context.Sut.CreateSnapshotOfContext(expectedContainerA);
			Assert.AreNotSame(expectedContainerA,originalContainerA);

			var expectedContainerB = Context.Sut.CreateNew();
			expectedContainerB.ContainerFlowHandler = expectedContainerA.FlowHandler;
			var stepDataB = expectedContainerB.AddStepData("stepDataB", true);
			var originalContainerB = expectedContainerB;
			expectedContainerB =await Context.Sut.CreateSnapshotOfContext(expectedContainerB);
			Assert.AreNotSame(expectedContainerB, originalContainerB);


			var expectedContainerC = Context.Sut.CreateNew();
			expectedContainerC.ContainerFlowHandler = expectedContainerB.FlowHandler;
			var stepDataC = expectedContainerC.AddStepData("stepDataC", true);
			
			var originalContainerC = expectedContainerC;
			var newC=await Context.Sut.CreateSnapshotOfContext(expectedContainerC);
			Assert.AreNotSame(newC, originalContainerC);
			var newB = await Context.Sut.Get(newC.ContainerFlowHandler);
			var newA= await Context.Sut.Get(newB.ContainerFlowHandler);
			
			var newBStepData = newB.GetCurrentStepData<UiFlowScreenModel>().Metadata;
			var newAStepData = newA.GetCurrentStepData<UiFlowScreenModel>().Metadata;
			
			Assert.AreEqual(newAStepData.ContainedFlowHandler,newB.FlowHandler);
			Assert.AreEqual(newBStepData.ContainedFlowHandler, newC.FlowHandler);
			Assert.AreEqual(newBStepData.FlowHandler, newC.ContainerFlowHandler);
			Assert.AreEqual(newAStepData.FlowHandler, newB.ContainerFlowHandler);
			Assert.IsNull(newA.ContainerFlowHandler);


			originalContainerA = await Context.Sut.Get(originalContainerA.FlowHandler);
			var actualStepA = originalContainerA.GetStepData<UiFlowScreenModel>(stepDataA.FlowScreenName).Metadata;
			Assert.AreEqual(actualStepA.FlowHandler,stepDataA.FlowHandler);
			Assert.AreEqual(actualStepA.ContainedFlowHandler, stepDataA.Metadata.ContainedFlowHandler);


		}

	

		[Test]
		public async Task LoadByFlowHandler_ItThrowsWhenNotExists()
		{
			Assert.ThrowsAsync<KeyNotFoundException>(async ()=> await Context.Sut.Get(Guid.NewGuid().ToString()));
		}

		[Test]
		public async Task ItCan_GetRootContainerContext_WhenIsContained()
		{
			var expectedContainer = Context.Sut.CreateNew();
			var stepDataContainer = expectedContainer.AddStepData("TestStepNameContainer", true);
			

			var expectedContained = Context.Sut.CreateNew();
			expectedContained.ContainerFlowHandler = expectedContainer.FlowHandler;


			var stepDataContained=expectedContainer.AddStepData("TestStepNameContained");
			
			var actual = await Context.Sut.GetRootContainerContext(expectedContained);
			
			Assert.AreEqual(expectedContainer.GetCurrentStepData<UiFlowScreenModel>() , actual.GetCurrentStepData<UiFlowScreenModel>());
		}

		[Test]
		public async Task ItCan_GetRootContainerContext_WhenIsRoot()
		{
			var expected = Context.Sut.CreateNew();
			var stepData = expected.AddStepData("TestStepName1");
			
			var actual = await Context.Sut.GetRootContainerContext(expected);

			Assert.AreSame(expected,actual);
		}

		

		public class TestContext : UnitTestContext<UiFlowContextDataRepositoryDecorator>
		{
			public DefaultRepositoryAdapter DefaultRepositoryAdapter { get; } =
				new DefaultRepositoryAdapter(new InMemoryFlowsStore());

			protected override UiFlowContextDataRepositoryDecorator BuildSut(AutoMocker autoMocker)
			{
				autoMocker.Use<IRepositoryAdapter>(DefaultRepositoryAdapter);
				return base.BuildSut(autoMocker);
			}
		}


	}
}
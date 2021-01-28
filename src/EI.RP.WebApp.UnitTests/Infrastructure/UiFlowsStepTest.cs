using System;
using EI.RP.TestServices;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Infrastructure
{
	[TestFixture]
	abstract class
		UiFlowsStepTest<TSut, TFlowType> : UnitTestFixture<UiFlowsStepTest<TSut, TFlowType>.TestContext, TSut>
		where TSut : UiFlowScreen<TFlowType>
	{
		public class TestContext : UnitTestContext<TSut>
		{
			private Navigation[] _navigations = new Navigation[0];
			private ScreenName _step;

			public TestContext() : base(mockBehavior: MockBehavior.Strict)
			{
			}

			public UiFlowContextData ContextData => new UiFlowContextData();

			protected override TSut BuildSut(AutoMocker autoMocker)
			{
				var mock = autoMocker.GetMock<IInternalScreenFlowConfigurator>();
				mock.Setup(x => x.AddErrorTransitionIfUndefined());
				foreach (var navigation in _navigations)
				{
					if (navigation.NavigatesTo == _step)
					{
						mock.Setup(x => x.OnEventReentriesCurrent(It.Is<ScreenEvent>(p => p == navigation.OnEvent)))
							.Returns(() => mock.Object).Verifiable();
					}
					else
					{
						mock.Setup(x => x.OnEventNavigatesTo(It.Is<ScreenEvent>(p => p == navigation.OnEvent),
								It.Is<ScreenName>(s => s == navigation.NavigatesTo)))
							.Returns(() => mock.Object).Verifiable();
					}

				}

				//check calls count to be the navigations
				return base.BuildSut(autoMocker);
			}


			public TestContext WithNavigations(Navigation[] expectedNavigations)
			{
				_navigations = expectedNavigations;
				return this;

			}

			public void SetStep(ScreenName step)
			{
				_step = step;
			}
		}

		public override void SetUp()
		{
			base.SetUp();
			Context.SetStep(ExpectedScreenStep);
		}

		[Test]
		public void IsWellFormed()
		{
			Assert.AreEqual(ExpectedToBeIncludedInFlowType, Context.Sut.IncludedInFlowType);

			Assert.IsTrue(object.ReferenceEquals(ExpectedScreenStep, Context.Sut.ScreenStep));


			Assert.IsTrue(string.Equals(ExpectedViewPath, Context.Sut.ViewPath,
				StringComparison.InvariantCultureIgnoreCase));
		}


		public abstract string ExpectedViewPath { get; }

		protected abstract TFlowType ExpectedToBeIncludedInFlowType { get; }

		protected abstract ScreenName ExpectedScreenStep { get; }


		[Ignore("TODO")]
		[Test]
		public void EventTransitionsAreCorrect()
		{
			Context
				.WithNavigations(ExpectedNavigations)
				.Sut
				.DefineTransitionsFromCurrentScreen(Context.AutoMocker.GetMock<IInternalScreenFlowConfigurator>().Object,
					Context.ContextData);



			Context.VerifyOnlyVerifiableExpectations();
		}

		protected abstract Navigation[] ExpectedNavigations { get; }

		public class Navigation
		{
			public Navigation(ScreenEvent onEvent, ScreenName navigatesTo)
			{
				OnEvent = onEvent;
				NavigatesTo = navigatesTo;
			}

			public ScreenEvent OnEvent { get; }
			public ScreenName NavigatesTo { get; }
		}
		[Ignore("TODO")]
		[Test]
		public void CanHandlesStepEvents()
		{
			throw new NotImplementedException();
		}
		[Ignore("TODO")]
		[Test]
		public void CanValidateTransitionAttempts()
		{
			throw new NotImplementedException();
		}
		[Ignore("TODO")]
		[Test]
		public void CanRefreshStepData()
		{
			throw new NotImplementedException();
		}
		[Ignore("TODO")]
		[Test]
		public void CanCreateStepData()
		{
			throw new NotImplementedException();
		}

	}
}
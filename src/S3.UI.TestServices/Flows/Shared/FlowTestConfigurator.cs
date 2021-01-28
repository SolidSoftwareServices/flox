using System;
using System.Diagnostics;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Moq.AutoMock;

namespace S3.UI.TestServices.Flows.Shared
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public abstract class FlowTestConfigurator<TConfigurator, TAdapter> 
		where TConfigurator : FlowTestConfigurator<TConfigurator,TAdapter>
		where TAdapter:class,IFlowComponentAdapter
	{
		protected FlowTestConfigurator(IFixture fixture)
		{
			Fixture = fixture;
			Fixture = Fixture.Customize(new AutoMoqCustomization());
			Mocker = new AutoMocker();
		}

		protected AutoMocker Mocker { get; }
		protected IFixture Fixture { get; }


		private TAdapter _adapter = default(TAdapter);

		/// <summary>
		/// The adapter represents the test flow component
		/// </summary>
		public TAdapter Adapter => _adapter ?? (_adapter = BuildAdapter());

		protected abstract TAdapter BuildAdapter();


		public TConfigurator WithStub<TStub>(TStub stub)
		{
			ThrowIfAdapterAlreadyCreated();
			foreach (var serviceType in typeof(TStub).GetInterfaces())
			{
				Mocker.Use(serviceType, stub);
			}

			return (TConfigurator)this;
		}

		public TConfigurator WithMockConfiguration<TMockedService>(Action<Mock<TMockedService>> mockConfigurator) where TMockedService : class
		{
			ThrowIfAdapterAlreadyCreated();
			mockConfigurator(Mocker.GetMock<TMockedService>());
			return (TConfigurator)this;
		}
		public TConfigurator WithStubConfiguration<TMockedService>(Action<TMockedService> stubConfigurator) where TMockedService : class
		{
			ThrowIfAdapterAlreadyCreated();
			stubConfigurator(Mocker.Get<TMockedService>());
			return (TConfigurator)this;
		}
		private void ThrowIfAdapterAlreadyCreated()
		{
			if (_adapter != null) throw new InvalidOperationException("The adapter was already created");
		}

	}
}
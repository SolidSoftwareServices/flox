using System;
using System.Diagnostics;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Moq.AutoMock;

namespace S3.TestServices
{
	/// <summary>
	/// Base class for unit tests context. An specific unit tests object mother
	/// </summary>
	/// <typeparam name="TSut"></typeparam>
	[DebuggerStepThrough]
	public class UnitTestContext<TSut> : IDisposable where TSut : class
	{
		public UnitTestContext() : this(null)
		{

		}

		public UnitTestContext(MockBehavior mockBehavior = MockBehavior.Default) : this(null, mockBehavior)
		{

		}

		public UnitTestContext(IFixture fixture = null):this(fixture,MockBehavior.Default)
		{

		}
		public UnitTestContext(IFixture fixture = null,MockBehavior mockBehavior=MockBehavior.Default)
		{
			Fixture = fixture ?? new Fixture();
			Fixture = Fixture.Customize(new AutoMoqCustomization());
			AutoMocker = new AutoMocker(mockBehavior);
		}

		
		private TSut _sut;
		public AutoMocker AutoMocker { get; private set; } 
		public IFixture Fixture { get; }

		/// <summary>
		/// System Under Test. the target of the current unit tests
		/// </summary>
		public TSut Sut
		{
			get
			{
				if (_sut == null)
				{
					_sut = BuildSut(AutoMocker);
				}
				return _sut;
			}
		}

		protected virtual TSut BuildSut(AutoMocker autoMocker)
		{
			return autoMocker.CreateInstance<TSut>();
		}

		public virtual void Reset()
		{
			if (_sut is IDisposable disposable)
			{
				disposable.Dispose();
			}
			_sut = null;
			AutoMocker = new AutoMocker(AutoMocker.MockBehavior);
		}

		public virtual void VerifyAllExpectations()
		{
			AutoMocker.VerifyAll();
		}

		public virtual void VerifyOnlyVerifiableExpectations()
		{
			AutoMocker.Verify();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			(Sut as IDisposable)?.Dispose();
		}
		~UnitTestContext()
		{
			Dispose(false);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Components.MovingHouseHeader;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Components.MovingHouseHeader
{
	[TestFixture]
	class MovingHouseHeaderComponent_ViewBuilder_Tests:UnitTestFixture<MovingHouseHeaderComponent_ViewBuilder_Tests.TestContext, Component.ViewModelBuilder>
	{

		public static IEnumerable<TestCaseData> Resolve_ItSetsHeader_Correctly_Cases()
		{

			yield return new TestCaseData(MovingHouseType.MoveElectricityAndCloseGas).Returns("Moving Electricity & Closing Gas Account");
			yield return new TestCaseData(MovingHouseType.CloseElectricityAndGas).Returns("Closing Electricity & Gas Accounts");
			yield return new TestCaseData(MovingHouseType.MoveElectricityAndGas).Returns("Moving Electricity & Gas Accounts");
			yield return new TestCaseData(MovingHouseType.MoveElectricity).Returns("Moving Electricity Account");
			yield return new TestCaseData(MovingHouseType.CloseGas).Returns("Closing Gas Account");
			yield return new TestCaseData(MovingHouseType.CloseElectricity).Returns("Closing Electricity Account");
			yield return new TestCaseData(MovingHouseType.MoveElectricityAndAddGas).Returns("Moving Electricity & Adding Gas Account");
			
			yield return new TestCaseData(MovingHouseType.MoveGasAndAddElectricity).Returns("Moving Gas & Adding Electricity Account");

		}
		[TestCaseSource(nameof(Resolve_ItSetsHeader_Correctly_Cases))]
		public string Resolve_ItSetsHeader_Correctly(MovingHouseType movingHouseType)
		{
			return Context.WithMovingHouse(movingHouseType)
				.Sut
				.Resolve(Context.BuildInput())
				.GetAwaiter().GetResult()
				.HeaderText;
		}

		public static IEnumerable<TestCaseData> Resolve_ItSetsPrnText_Correctly_Cases()
		{
			yield return new TestCaseData(MovingHouseType.MoveElectricityAndCloseGas).Returns("MPRN");
			yield return new TestCaseData(MovingHouseType.CloseElectricityAndGas).Returns(null);
			yield return new TestCaseData(MovingHouseType.MoveElectricityAndGas).Returns("MPRN/GPRN");
			yield return new TestCaseData(MovingHouseType.MoveElectricity).Returns("MPRN");
			yield return new TestCaseData(MovingHouseType.CloseGas).Returns(null);
			yield return new TestCaseData(MovingHouseType.CloseElectricity).Returns(null);
			yield return new TestCaseData(MovingHouseType.MoveElectricityAndAddGas).Returns("MPRN/GPRN");

			yield return new TestCaseData(MovingHouseType.MoveGasAndAddElectricity).Returns("MPRN/GPRN");

		}
		[TestCaseSource(nameof(Resolve_ItSetsPrnText_Correctly_Cases))]
		public string Resolve_ItSetsPrnText_Correctly(MovingHouseType movingHouseType)
		{
			return Context.WithMovingHouse(movingHouseType)
				.Sut
				.Resolve(Context.BuildInput())
				.GetAwaiter().GetResult()
				.PrnText;
		}

		[Test]
		public void Resolve_ItSetsCurrentStepNumber_Correctly([Range(0,6)]int stepNumber)
		{
			var actual= Context.WithStepNumber(stepNumber)
				.Sut
				.Resolve(Context.BuildInput())
				.GetAwaiter().GetResult()
				.CurrentStepNumber;
			Assert.AreEqual(stepNumber,actual);
		}

		[TestCase(-1)]
		[TestCase(7)]
		public void Resolve_ItThrowsIfInvalidCurrentStepNumber(int stepNumber)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				var a = Context.WithStepNumber(stepNumber)
					.Sut
					.Resolve(Context.BuildInput())
					.GetAwaiter().GetResult()
					.CurrentStepNumber;
			});
		}


		public static IEnumerable<TestCaseData> Resolve_ItShowsProcess_Correctly_Cases()
		{
			return Enumerable.Range(1, 5).Select(stepNumber => new TestCaseData(stepNumber).Returns(true)).Union(new[]
				{new TestCaseData(0).Returns(false), new TestCaseData(6).Returns(false)});
		}

		[TestCaseSource(nameof(Resolve_ItShowsProcess_Correctly_Cases))]
		public bool Resolve_ItShowsProcess_Correctly(int stepNumber)
		{
			return Context.WithStepNumber(stepNumber)
				.Sut
				.Resolve(Context.BuildInput())
				.GetAwaiter().GetResult()
				.ShowProcess;
		}

		public class TestContext : UnitTestContext<Component.ViewModelBuilder>
		{
			private MovingHouseType _movingHouseType=MovingHouseType.MoveGas;
			private int _stepNumber=1;

			public TestContext WithMovingHouse(MovingHouseType movingHouseType)
			{
				_movingHouseType = movingHouseType;
				return this;
			}

			public TestContext WithStepNumber(int stepNumber)
			{
				_stepNumber = stepNumber;
				return this;
			}
			public InputModel BuildInput()
			{
				return new InputModel
				{
					MovingHouseType = _movingHouseType
					,StepNumber=_stepNumber
				};
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class IsSapCheckMoveOutOkValidatorUnitTests : UnitTestFixture<
		IsSapCheckMoveOutOkValidatorUnitTests.TestContext,
		IsSapCheckMoveOutOkValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var isSapMoveOutOkTruthTable = new List<Tuple<bool, bool, OutputType>>()
			{
				new Tuple<bool, bool, OutputType>(
					false,
					false,
					OutputType.Failed),
				new Tuple<bool, bool, OutputType>(
					false,
					true,
					OutputType.Failed),
				new Tuple<bool, bool, OutputType>(
					true,
					false,
					OutputType.Failed),
				new Tuple<bool, bool, OutputType>(
					true,
					true,
					OutputType.Passed),
			};

			foreach (var testCase in isSapMoveOutOkTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.IsSapCheckMoveOutOk
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var discStatusIsNewTruthTable = new List<Tuple<bool, OutputType>>()
			{
				new Tuple<bool, OutputType>(false, OutputType.Failed),
				new Tuple<bool, OutputType>(true, OutputType.Passed),
			};

			foreach (var testCase in discStatusIsNewTruthTable)
			{
				var isValid = testCase.Item2;
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.IsSapCheckMoveOutOk
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(
			bool electricityMoveOutIsOk, bool gasMoveOutIsOk)
		{
			return await Context
				.WithElectricityAccount()
				.WithIsElectricityMoveOutOk(electricityMoveOutIsOk)
				.WithGasAccount()
				.WithIsGasMoveOutOk(gasMoveOutIsOk)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(bool isOk)
		{
			return await Context
				.WithElectricityAccount()
				.WithIsElectricityMoveOutOk(isOk)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(bool isOk)
		{
			return await Context
				.WithGasAccount()
				.WithIsGasMoveOutOk(isOk)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsSapCheckMoveOutOkValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private bool _isElectricityMoveOutOk;
			private bool _isGasMoveOutOk;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityAccount()
			{
				_addElectricity = true;
				return this;
			}

			public TestContext WithGasAccount()
			{
				_addGas = true;
				return this;
			}

			public TestContext WithIsElectricityMoveOutOk(bool isOk)
			{
				_isElectricityMoveOutOk = isOk;
				return this;
			}

			public TestContext WithIsGasMoveOutOk(bool isOk)
			{
				_isGasMoveOutOk = isOk;
				return this;
			}

			protected override IsSapCheckMoveOutOkValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_addElectricity)
				{
					cfg.AddElectricityAccount(isMoveOutOk: _isElectricityMoveOutOk)
						.WithElectricity24HrsDevices();
				}

				if (_addGas)
				{
					cfg.AddGasAccount(isMoveOutOk: _isGasMoveOutOk)
						.WithGasDevice();
				}

				cfg.Execute();

				if (_addElectricity)
				{
					var query = new CheckMoveOutRequestResultQuery
					{
						ContractID = cfg.ElectricityAccount().Model.ContractId
					};
				}

				if (_addGas)
				{
					var query = new CheckMoveOutRequestResultQuery
					{
						ContractID = cfg.GasAccount().Model.ContractId
					};
				}

				var electricityAccount = cfg.ElectricityAccount()?.Model;
				var gasAccount = cfg.GasAccount()?.Model;

				Query = Fixture.Build<MovingHouseValidationQuery>()
					.With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
					.With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
					.Without(x => x.MovingHouseType)
					.Create();

				return base.BuildSut(autoMocker);
			}
		}
	}
}
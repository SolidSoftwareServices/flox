using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using AutoFixture;
using NUnit.Framework;
using Moq;

namespace EI.RP.DomainServices.UnitTests.Commands.Metering.SubmitMeterReading
{
	internal class SubmitMeterReadingCommandHandlerTest : CommandHandlerTest<SubmitMeterReadingCommandHandler, SubmitMeterReadingCommand>
	{
		private const string MeteringSettingsTestRoloverValue = "90000";
		private const string AllowByPassToleranceCheckNoteID = "ZI01";
		private const decimal NoHistoryValue = -1;

		internal class MeterTypeAndSubmitValue
		{
			public MeterType MeterType { get; set; }
			public decimal SubmitValue { get; set; }
		}

		internal class RegisterAndSubmitValue
		{
			public DeviceRegisterInfo DeviceRegisterInfo { get; set; }
			public decimal SubmitValue { get; set; }
		}

		internal class CaseModel
		{
			public MeterTypeAndSubmitValue[] MeterTypeAndSubmitValues { get; set; }
			public ClientAccountType AccountType { get; set; }
			public bool IsSubmitGasMeterResult { get; set; }
			public decimal LastHistoryReadingsNetworkValue { get; set; }
			public decimal LastHistoryReadingsCustomerValue { get; set; }
			public bool IsNetworkResultLastInHistory { get; set; }
			public bool ShouldThrowSubmitNetworkHistoryError { get; set; }
			public bool ShouldThrowSubmitCustomerHistoryError { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 1000 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 500,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "Electricity1MeterWithHigherThanNetworkAndCustomerHistoryPassedAllRules"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 1000 },
														new MeterTypeAndSubmitValue { MeterType = MeterType.ElectricityNight, SubmitValue = 1200 }},
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 500,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "Electricity2MetersWithHigherThanNetworkAndCustomerHistoryPassedAllRules"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = 1000 } },
					AccountType = ClientAccountType.Gas,
					IsSubmitGasMeterResult = true,
					LastHistoryReadingsNetworkValue = 500,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "GasMeterWithHigherThanNetworkAndCustomerHistoryPassedAllRules"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = 400 } },
					AccountType = ClientAccountType.Gas,
					IsSubmitGasMeterResult = true,
					LastHistoryReadingsNetworkValue = 500,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = true,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "GasMeterWithLowerThanNetworkAndCustomerHistoryRule1Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = 600 } },
					AccountType = ClientAccountType.Gas,
					IsSubmitGasMeterResult = true,
					LastHistoryReadingsNetworkValue = 0,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "GasMeterWithWithLowerThanCustomerHistoryRule2Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = 600 } },
					AccountType = ClientAccountType.Gas,
					IsSubmitGasMeterResult = true,
					LastHistoryReadingsNetworkValue = -1,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = false,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "GasMeterWithLowerThanCustomerHistoryRule3Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 400 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 500,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = true,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "Electricity1MeterWithLowerThanCustomerHistoryRule1Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 600 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 0,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "Electricity1MeterWithLowerThanCustomerHistoryRule2Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 600 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = -1,
					LastHistoryReadingsCustomerValue = 700,
					IsNetworkResultLastInHistory = false,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "Electricity1MeterWithLowerThanCustomerHistoryRule3Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 200 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 90300,
					LastHistoryReadingsCustomerValue = 89700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "RolloverTestElectricityHistoryAllPassed"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 400 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 90300,
					LastHistoryReadingsCustomerValue = 89700,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = true,
					ShouldThrowSubmitCustomerHistoryError = false,
					CaseName = "RolloverTestElectricityHistoryRule1Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 400 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = 0,
					LastHistoryReadingsCustomerValue = 90300,
					IsNetworkResultLastInHistory = true,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "RolloverTestElectricityHistoryRule2Error"
				},
				new CaseModel {
					MeterTypeAndSubmitValues = new [] { new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = 400 } },
					AccountType = ClientAccountType.Electricity,
					IsSubmitGasMeterResult = false,
					LastHistoryReadingsNetworkValue = -1,
					LastHistoryReadingsCustomerValue = 90300,
					IsNetworkResultLastInHistory = false,
					ShouldThrowSubmitNetworkHistoryError = false,
					ShouldThrowSubmitCustomerHistoryError = true,
					CaseName = "RolloverTestElectricityHistoryRule3Error"
				},
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName($"TestHistoryResultValidator-{caseItem.CaseName}");
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task TestMeterLastResultsValidator(CaseModel caseModel)
		{
			MeterReadingResultDto meterReadingResultDto = null;
			ServiceOrderDto serviceOrderDto = null;

			var cmd = ArrangeAndGetCommand(caseModel.AccountType,
										  caseModel.IsSubmitGasMeterResult,
										  null,
										  false,
										  caseModel.MeterTypeAndSubmitValues,
										  ref meterReadingResultDto,
										  ref serviceOrderDto,
										  true,
										  caseModel.LastHistoryReadingsNetworkValue,
										  caseModel.LastHistoryReadingsCustomerValue,
										  caseModel.IsNetworkResultLastInHistory);

			var repo = Context.AutoMocker.GetMock<ISapRepositoryOfErpUmc>();
			repo.Verify(x => x.Add(It.IsAny<MeterReadingResultDto>(), It.IsAny<bool>()), Times.Never);
			repo.Verify(x => x.Add(It.IsAny<ServiceOrderDto>(), It.IsAny<bool>()), Times.Never);

			try
			{
				await Context.Sut.ExecuteAsync(cmd);
			}
			catch (DomainException domainException)
			{
				var isExpectedDomainError = (caseModel.ShouldThrowSubmitNetworkHistoryError && domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualNetwork)) ||
											(caseModel.ShouldThrowSubmitCustomerHistoryError && domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualCustomer));
				if (!isExpectedDomainError) throw;

			}

			var shouldThrowSubmitHistoryError = caseModel.ShouldThrowSubmitNetworkHistoryError ||
												caseModel.ShouldThrowSubmitCustomerHistoryError;

			if (!shouldThrowSubmitHistoryError)
			{
				AssertAdded();
			}
			else
			{
				AssertErrored();
			}

			repo.VerifyNoOtherCalls();

			void AssertAdded()
			{
				if (caseModel.IsSubmitGasMeterResult)
				{
					repo.Verify(x => x.Add(It.Is<ServiceOrderDto>(m => CompareServiceOrderDto(m, serviceOrderDto)), true), Times.Once);
				}
				else
				{
					repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), true), Times.Once);
				}
			}

			void AssertErrored()
			{
				repo.Verify(x => x.Add(It.Is<ServiceOrderDto>(m => CompareServiceOrderDto(m, serviceOrderDto)), true), Times.Never);
				repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), true), Times.Never);
			}
		}

		[Test]
		public async Task TestSubmitMeterReadingForAddGas()
		{
			var gprn = Context.Fixture.Create<GasPointReferenceNumber>();
			var metersAndSubmitValues = new List<MeterTypeAndSubmitValue>
			{
				new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = Context.Fixture.Create<decimal>() }
			};

			MeterReadingResultDto meterReadingResultDto = null;
			ServiceOrderDto serviceOrderDto = null;

			var cmd = ArrangeAndGetCommand(ClientAccountType.Electricity, false, gprn, true, metersAndSubmitValues.ToArray(), ref meterReadingResultDto, ref serviceOrderDto,  false);
			var repo = Context.AutoMocker.GetMock<ISapRepositoryOfErpUmc>();
			repo.Verify(x => x.Add(It.IsAny<MeterReadingResultDto>(), It.IsAny<bool>()), Times.Never);

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), true), Times.Once);
				repo.VerifyNoOtherCalls();
			}
		}

		[Test]
		public async Task TestSubmitMeterReadingGas()
		{
			var metersAndSubmitValues = new List<MeterTypeAndSubmitValue>
			{
				new MeterTypeAndSubmitValue { MeterType = MeterType.Gas, SubmitValue = Context.Fixture.Create<decimal>() }
			};
			MeterReadingResultDto meterReadingResultDto = null;
			ServiceOrderDto serviceOrderDto = null;

			var cmd = ArrangeAndGetCommand(ClientAccountType.Gas, true, null, false, metersAndSubmitValues.ToArray(), ref meterReadingResultDto, ref serviceOrderDto, false);
			var repo = Context.AutoMocker.GetMock<ISapRepositoryOfErpUmc>();
			repo.Verify(x => x.Add(It.IsAny<MeterReadingResultDto>(), It.IsAny<bool>()), Times.Never);
			repo.Verify(x => x.Add(It.IsAny<ServiceOrderDto>(), It.IsAny<bool>()), Times.Never);

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), true), Times.Never);
				repo.Verify(x => x.Add(It.Is<ServiceOrderDto>(m => CompareServiceOrderDto(m, serviceOrderDto)), true), Times.Once);
				repo.VerifyNoOtherCalls();
			}
		}

		private bool CompareMeterReading(MeterReadingResultDto left, MeterReadingResultDto right)
		{
			if (left.DependentMeterReadingResults.Count() != right.DependentMeterReadingResults.Count())
				return false;

			var compareRoot = CompareMeterReadingDto(left, right);
			var compareDependentMeterReading = true;
			var index = 0;

			foreach(var dependentMeterReading in left.DependentMeterReadingResults)
			{
				compareDependentMeterReading = compareDependentMeterReading &&
					CompareMeterReadingDto(dependentMeterReading, right.DependentMeterReadingResults[index]);
				index++;
			}

			return compareRoot && compareDependentMeterReading;
		}

		private bool CompareMeterReadingDto(MeterReadingResultDto left, MeterReadingResultDto right)
		{
			return left.MeterReadingResultID == right.MeterReadingResultID &&
				   left.DeviceID == right.DeviceID &&
				   left.RegisterID == right.RegisterID &&
				   left.ReadingResult == right.ReadingResult &&
				   left.ReadingDateTime == right.ReadingDateTime &&
				   left.MeterReadingReasonID == right.MeterReadingReasonID &&
				   left.Lcpe == right.Lcpe &&
				   left.MeterReadingCategoryID == right.MeterReadingCategoryID &&
				   left.SerialNumber == right.SerialNumber &&
				   left.MeterReadingNoteID == right.MeterReadingNoteID &&
				   left.Consumption == right.Consumption &&
				   left.Email == right.Email &&
				   left.FmoRequired == right.FmoRequired &&
				   left.Vkont == right.Vkont &&
				   left.Vertrag == right.Vertrag &&
				   left.PchaRequired == right.PchaRequired &&
				   left.ReadingUnit == right.ReadingUnit &&
				   left.MeterReadingStatusID == right.MeterReadingStatusID &&
				   left.SkipUsrCheck == right.SkipUsrCheck &&
				   left.MultipleMeterReadingReasonsFlag == right.MultipleMeterReadingReasonsFlag;
		}

		private bool CompareServiceOrderDto(ServiceOrderDto left, ServiceOrderDto right)
		{
			return left.ServiceOrderID == right.ServiceOrderID &&
				   left.ServiceOrderTypeID == right.ServiceOrderTypeID &&
				   left.StartDate == right.StartDate &&
				   left.EndDate == right.EndDate &&
				   left.ServicePriorityID == right.ServicePriorityID &&
				   left.Description == right.Description &&
				   left.Note == right.Note &&
				   left.ServiceNotificationID == right.ServiceNotificationID &&
				   left.DeviceID == right.DeviceID &&
				   left.AccountID == right.AccountID &&
				   left.SystemStatus == right.SystemStatus &&
				   left.Status == right.Status &&
				   left.ConnectionObjectID == right.ConnectionObjectID &&
				   left.CompanyCode == right.CompanyCode &&
				   left.MeterRead == right.MeterRead &&
				   left.WorkCentre == right.WorkCentre;
		}

		private string GetMeterReadingType(string meterTypeName)
		{
			return meterTypeName.Equals(MeterType.Gas.ToString()) ? MeterUnit.M3 : MeterUnit.KWH;
		}

		private RegisterAndSubmitValue[] ArrageRegisters(MeterTypeAndSubmitValue[] meterAndSubmitValues)
		{
			var deviceId = Context.Fixture.Create<long>().ToString();
			var registerAndSubmitInfos = new List<RegisterAndSubmitValue>();

			foreach (var meterAndSubmitValue in meterAndSubmitValues)
			{
				var registerAndSubmitValue = new RegisterAndSubmitValue();
				var registerInfos = Context.Fixture.Build<DeviceRegisterInfo>()
					.With(x => x.RegisterId, MeterReadingRegisterType.ActiveEnergyRegisterType)
					.With(x => x.MeterType, meterAndSubmitValue.MeterType)
					.With(x => x.MeterNumber, Context.Fixture.Create<long>().ToString())
					.With(x => x.MeterUnit, meterAndSubmitValue.MeterType == MeterType.Gas ? MeterUnit.M3 : MeterUnit.KWH)
					.Create();

				registerAndSubmitValue.DeviceRegisterInfo = registerInfos;
				registerAndSubmitValue.SubmitValue = meterAndSubmitValue.SubmitValue;

				registerAndSubmitInfos.Add(registerAndSubmitValue);
			}		

			return registerAndSubmitInfos.ToArray();
		}

		[Test]
		public async Task TestSubmitMeterReadingElectricityWithExpectedValues()
		{
			var metersAndSubmitValues = new List<MeterTypeAndSubmitValue>
			{
				new MeterTypeAndSubmitValue { MeterType = MeterType.Electricity24h, SubmitValue = Context.Fixture.Create<decimal>() }
			};
			MeterReadingResultDto meterReadingResultDto = null;
			ServiceOrderDto serviceOrderDto = null;

			var cmd = ArrangeAndGetCommand(ClientAccountType.Electricity, false, null, false, metersAndSubmitValues.ToArray(), ref meterReadingResultDto, ref serviceOrderDto, true);
			var repo = Context.AutoMocker.GetMock<ISapRepositoryOfErpUmc>();
			repo.Verify(x => x.Add(It.IsAny<MeterReadingResultDto>(), It.IsAny<bool>()), Times.Never);

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), true), Times.Once);
				repo.VerifyNoOtherCalls();
			}
		}

		[Test]
		public async Task TestSubmitElectricityMeterReadingWithHigherThanExpectedValues()
		{
			var meterTypes = new MeterType[] { MeterType.Electricity24h };
			await AssertTestSubmitMeterReadingWithHigherThanExpectedValues(meterTypes, ClientAccountType.Electricity);
		}

		[Test]
		public async Task TestSubmitElectricitySubmitManyMeterReadingWithHigherThanExpectedValues()
		{
			var meterTypes = new MeterType[] { MeterType.Electricity24h, MeterType.ElectricityNightStorageHeater };
			await AssertTestSubmitMeterReadingWithHigherThanExpectedValues(meterTypes, ClientAccountType.Electricity);
		}

		private async Task AssertTestSubmitMeterReadingWithHigherThanExpectedValues(MeterType[] meterTypes, ClientAccountType clientAccountType)
		{
			MeterReadingResultDto meterReadingResultDto = null;
			ServiceOrderDto serviceOrderDto = null;

			var metersAndSubmitValues = new List<MeterTypeAndSubmitValue>();
			foreach(var meterType in meterTypes)
			{
				var meterAndSubmitValue = new MeterTypeAndSubmitValue { MeterType = meterType, SubmitValue = Context.Fixture.Create<decimal>() };
				metersAndSubmitValues.Add(meterAndSubmitValue);
			}

			var cmd = ArrangeAndGetCommand(clientAccountType, false, null, false, metersAndSubmitValues.ToArray(), ref meterReadingResultDto, ref serviceOrderDto, true);
			var repo = Context.AutoMocker.GetMock<ISapRepositoryOfErpUmc>();
			repo.Verify(x => x.Add(It.IsAny<MeterReadingResultDto>(), It.IsAny<bool>()), Times.Never);

			repo.Setup(x => x.Add(It.Is<MeterReadingResultDto>(m => m.MeterReadingNoteID == ""), It.IsAny<bool>()))
				.Throws(new DomainException(ResidentialDomainError.MeterSubmitOutOfTolerance));

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				meterReadingResultDto.MeterReadingNoteID = AllowByPassToleranceCheckNoteID;
				foreach (var dependentMeterReadingResult in meterReadingResultDto.DependentMeterReadingResults)
				{
					dependentMeterReadingResult.MeterReadingNoteID = AllowByPassToleranceCheckNoteID;
				}
				repo.Verify(x => x.Add(It.Is<MeterReadingResultDto>(m => CompareMeterReading(m, meterReadingResultDto)), It.IsAny<bool>()), Times.Exactly(2));
				repo.VerifyNoOtherCalls();
			}
		}

		private SubmitMeterReadingCommand ArrangeAndGetCommand(ClientAccountType clientAccountType,
															   bool isSubmitGasMeterResult,
															   GasPointReferenceNumber gprn,
															   bool isAddGas,
															   MeterTypeAndSubmitValue[] meterAndSubmitValues,
															   ref MeterReadingResultDto meterReadingResultDto,
															   ref ServiceOrderDto serviceOrderDto,
															   bool validateLastResults = false,
															   decimal lastHistoryReadingsNetworkValue = NoHistoryValue,
															   decimal lastHistoryReadingsCustomerValue= NoHistoryValue,
															   bool isNetworkResultLastInHistory = false)
		{
			var accountNumber = Context.Fixture.Create<long>().ToString();
			var meteringSettingsMock = Context.AutoMocker.GetMock<IMeteringSettings>();
			meteringSettingsMock.Setup(x => x.SubmitMeterRolloverValue)
								.Returns(MeteringSettingsTestRoloverValue);

			var registersAndSubmitValues = ArrageRegisters(meterAndSubmitValues);

			var device = Context
				.Fixture
				.Build<DeviceInfo>()
				.With(x => x.Registers, registersAndSubmitValues.Select(x=>x.DeviceRegisterInfo))
				.Create();

			var meterReadingDataResults = new List<MeterReadingData>();
			foreach (var registersAndSubmit in registersAndSubmitValues)
			{
				var meterReadingData = new MeterReadingData
				{
					DeviceId = device.DeviceId,
					MeterReading = registersAndSubmit.SubmitValue.ToString(),
					RegisterId = registersAndSubmit.DeviceRegisterInfo.RegisterId,
					MeterNumber = registersAndSubmit.DeviceRegisterInfo.MeterNumber,
					MeterTypeName = registersAndSubmit.DeviceRegisterInfo.MeterType.ToString(),
				};

				meterReadingData.FmoRequired = SetFmoRequired(device, meterReadingData)?.FmoRequired;
				meterReadingDataResults.Add(meterReadingData);
			}

			var accountInfo = Context
				.Fixture
				.Build<AccountInfo>()
				.With(x => x.AccountNumber, accountNumber)
				.With(x => x.ClientAccountType, clientAccountType)
				.With(x => x.PointReferenceNumber, clientAccountType.Equals(ClientAccountType.Electricity) ? 
													(PointReferenceNumber)Context.Fixture.Create<ElectricityPointReferenceNumber>() :
													(PointReferenceNumber)Context.Fixture.Create<GasPointReferenceNumber>())
				// test perfomance improvement
				.Without(x => x.BankAccounts)
				.Without(x => x.BusinessAgreement)
				.Without(x => x.NonSmartPeriods)
				.Without(x => x.SmartPeriods)
				.Create();

			var domainFacade = new DomainFacade().SetUpMocker(Context.AutoMocker);
			domainFacade.QueryResolver.ExpectQuery(
				new AccountInfoQuery
				{
					AccountNumber = accountNumber
				},
				accountInfo.ToOneItemArray().AsEnumerable());

			var historicMeterReadingResult = PrepareMeterReadingsHistoryData(accountNumber,
																	  accountInfo.ClientAccountType,
																	  meterReadingDataResults,
																	  lastHistoryReadingsNetworkValue,
																	  lastHistoryReadingsCustomerValue,
																	  isNetworkResultLastInHistory);

			domainFacade.QueryResolver.ExpectQuery(new MeterReadingsQuery
			{
				AccountNumber = accountNumber
			}, historicMeterReadingResult);

			if (isSubmitGasMeterResult)
			{
				serviceOrderDto = BuildServiceOrderDto(meterReadingDataResults.First());
			} else
			{
				foreach (var item in meterReadingDataResults)
				{
					var newMeter = BuildMeterReadingResultDto(item);
					if (meterReadingResultDto == null)
						meterReadingResultDto = newMeter;
					else
						meterReadingResultDto.DependentMeterReadingResults.Add(newMeter);
				}
			}

			return new SubmitMeterReadingCommand(accountNumber, meterReadingDataResults, gprn, false, isAddGas, validateLastResults);

			MeterReadingResultDto BuildMeterReadingResultDto(MeterReadingData meterReadingData)
			{
				const string LcpeForAddGas = "MOVEI";
				var defaultReadingUnit = meterReadingData.MeterTypeName.Equals(MeterType.Gas.ToString()) ? MeterUnit.M3 : MeterUnit.KWH;
				var lastHistoricMeterReadingResult = historicMeterReadingResult?.FirstOrDefault(x =>
						x.SerialNumber == meterReadingData.MeterNumber && x.RegisterId == meterReadingData.RegisterId);
				var fmoRequired = string.Empty;
				var lcpe = string.Empty;

				if (isAddGas)
				{
					lcpe = LcpeForAddGas;
					fmoRequired = (SapBooleanFlag)meterReadingData.FmoRequired;
				}
				else
				{
					lcpe = meterReadingData.Lcpe ?? lastHistoricMeterReadingResult?.Lcpe ?? string.Empty;
					fmoRequired = (SapBooleanFlag)meterReadingData.FmoRequired ?? lastHistoricMeterReadingResult?.FmoRequired ?? string.Empty;
				}

				var result = Context
					.Fixture
					.Build<MeterReadingResultDto>()
					.With(x => x.MeterReadingResultID, string.Empty)
					.With(x => x.DeviceID, meterReadingData.DeviceId)
					.With(x => x.RegisterID, meterReadingData.RegisterId)
					.With(x => x.ReadingResult, meterReadingData.MeterReading)
					.With(x => x.ReadingDateTime, meterReadingData.ReadingDateTime)
					.With(x => x.MeterReadingReasonID,
						!string.IsNullOrEmpty(meterReadingData.MeterReadingReasonID) ? meterReadingData.MeterReadingReasonID : MeterReadingReason.InterimMeterReadingWithoutBilling)
					.With(x => x.Lcpe, lcpe)
					.With(x => x.MeterReadingCategoryID, MeterReadingCategoryType.Customer)
					.With(x => x.SerialNumber, meterReadingData.MeterNumber)
					.With(x => x.MeterReadingNoteID, string.Empty)
					.With(x => x.Consumption, lastHistoricMeterReadingResult?.Consumption.ToString(CultureInfo.InvariantCulture))
					.With(x => x.Email, lastHistoricMeterReadingResult?.Email ?? string.Empty)
					.With(x => x.FmoRequired, fmoRequired)
					.With(x => x.Vkont, lastHistoricMeterReadingResult?.Vkont ?? string.Empty)
					.With(x => x.Vertrag, lastHistoricMeterReadingResult?.Vertrag ?? string.Empty)
					.With(x => x.PchaRequired, lastHistoricMeterReadingResult?.PchaRequired ?? string.Empty)
					.With(x => x.ReadingUnit, lastHistoricMeterReadingResult?.ReadingUnit ?? defaultReadingUnit)
					.With(x => x.MeterReadingStatusID, string.Empty)
					.With(x => x.SkipUsrCheck, string.Empty)
					.With(x => x.MultipleMeterReadingReasonsFlag, default(bool?))
					.Create();

				return result;
			}

			ServiceOrderDto BuildServiceOrderDto(MeterReadingData meterReadingData)
			{
				var result = Context
							.Fixture
							.Build<ServiceOrderDto>()
							.With(x => x.ServiceOrderID, "")
							.With(x => x.ServiceOrderTypeID, ServiceOrderType.Zm71)
							.With(x => x.StartDate, DateTimeOffset.UtcNow.Date)
							.With(x => x.EndDate, DateTimeOffset.UtcNow.Date)
							.With(x => x.ServicePriorityID, "")
							.With(x => x.Description, "GAS: Customer Read")
							.With(x => x.Note, "")
							.With(x => x.ServiceNotificationID, "")
							.With(x => x.DeviceID, "")
							.With(x => x.AccountID, accountInfo.Partner)
							.With(x => x.SystemStatus, "")
							.With(x => x.Status, "")
							.With(x => x.ConnectionObjectID, accountInfo.PremiseConnectionObjectId)
							.With(x => x.CompanyCode, ServiceOrderCompanyCode.SPLY)
							.With(x => x.MeterRead, meterReadingData.MeterReading)
							.With(x => x.WorkCentre, "NETWORKS")
							.Create();

				return result;
			}
		}

		private static MeterReadingData SetFmoRequired(DeviceInfo deviceInfo, MeterReadingData meterReadingData)
		{
			var meterReadMoveIn = deviceInfo.MeterReadingResults.Where(x => x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveIn).LastOrDefault();
			var meterReadMoveOut = deviceInfo.MeterReadingResults.Where(x => x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut).LastOrDefault();

			if (deviceInfo.MeterReadingResults.Last().MeterReadingReasonID ==
				MeterReadingReason.MeterReadingAtBillingRelInst)
			{
				throw new DomainException(ResidentialDomainError.CantProcessMoveInMoveOut,
					$"Last meter reading moving in  reason MeterReadingAtBillingRelInst.");
			}

			if (meterReadMoveIn == null && meterReadMoveOut == null
			|| meterReadMoveOut == null
			|| !(meterReadMoveIn == null || meterReadMoveOut.ReadingDateTime > meterReadMoveIn.ReadingDateTime))
			{
				meterReadingData.FmoRequired = true;
			}

			return meterReadingData;
		}

		private IEnumerable<MeterReadingInfo> PrepareMeterReadingsHistoryData(string accountNumber,
																			   ClientAccountType accountType,
																			   List<MeterReadingData> meterReadings,
																			   decimal lastHistoryReadingsNetworkValue,
																			   decimal lastHistoryReadingsCustomerValue,
																			   bool isNetworkResultLastInHistory)
		{
			var lastNetworkHistoryDate = DateTime.Today.AddDays(-2);
			var lastCustomerHistoryDate = isNetworkResultLastInHistory ? lastNetworkHistoryDate.AddDays(-1) : lastNetworkHistoryDate.AddDays(1);

			var historyMeterReadingDataNetwork = CreateMeterReadingHistory(accountType, 
																		   meterReadings, 
																		   MeterReadingCategoryType.Network, 
																		   lastHistoryReadingsNetworkValue, 
																		   lastNetworkHistoryDate);

			var historyMeterReadingDataCustomer = CreateMeterReadingHistory(accountType,
																		   meterReadings,
																		   MeterReadingCategoryType.Customer,
																		   lastHistoryReadingsCustomerValue,
																		   lastCustomerHistoryDate);

			var historyMeterReading = historyMeterReadingDataNetwork.Concat(historyMeterReadingDataCustomer);
			return historyMeterReading;
		}

		private IEnumerable<MeterReadingInfo> CreateMeterReadingHistory(ClientAccountType accountType,
																	   List<MeterReadingData> meterReadings,
																	   MeterReadingCategoryType meterReadingCategory,
																	   decimal historyReadingValue,
																	   DateTime historyReadingDate)
		{

			if (historyReadingValue == NoHistoryValue) return new MeterReadingInfo[0];

			var meterReadingHistoryResults = new List<MeterReadingInfo>();
			foreach (var meterReading in meterReadings)
			{
				var meterReadingHistory = Context.Fixture.Build<MeterReadingInfo>()
					.With(x => x.AccountType, accountType)
					.With(x => x.MeterType, MeterType.From(meterReading.MeterTypeName))
					.With(x => x.DeviceId, meterReading.DeviceId)
					.With(x => x.RegisterId, meterReading.RegisterId)
					.With(x => x.SerialNumber, meterReading.MeterNumber)
					.With(x => x.MeterReadingCategory, meterReadingCategory)
					.With(x => x.Reading, historyReadingValue)
					.With(x => x.ReadingDate, historyReadingDate)
					.Create();

				meterReadingHistoryResults.Add(meterReadingHistory);
			}
			return meterReadingHistoryResults;
		}
	}
}
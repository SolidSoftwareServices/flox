using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using Ei.Rp.DomainModels.Metering;
using NLog;

namespace EI.RP.DomainServices.Commands.Metering.SubmitMeterReading
{
	internal class SubmitMeterReadingCommandHandler : ICommandHandler<SubmitMeterReadingCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private const string LcpeForAddGas = "MOVEI";
		private const string AllowByPassToleranceCheckNoteID = "ZI01";
		private readonly ISapRepositoryOfErpUmc _dataRepository;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IMeteringSettings _meteringSettings;

		public SubmitMeterReadingCommandHandler(ISapRepositoryOfErpUmc dataRepository,
												IDomainQueryResolver queryResolver, 
												IDomainCommandDispatcher commandDispatcher,
												IMeteringSettings meteringSettings)
		{
			_dataRepository = dataRepository;
			_queryResolver = queryResolver;
			_commandDispatcher = commandDispatcher;
			_meteringSettings = meteringSettings;
		}

		public async Task ExecuteAsync(SubmitMeterReadingCommand command)
		{
			var accountInfo = string.IsNullOrEmpty(command.AccountNumber) ? null 
				: await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			if (accountInfo?.ClientAccountType == ClientAccountType.Electricity || command.NewPremisePrn != null || command.IsMovingOut())
			{
				await SubmitElectricityOrNewPremiseGasMeterReading(command, accountInfo);
			} else if (accountInfo?.ClientAccountType == ClientAccountType.Gas)
			{
				await SubmitGasMeterReading(command, accountInfo);
			} else
			{
				throw new NotSupportedException(
					$"accountInfo.AccountType={accountInfo?.ClientAccountType} is not supported");
			}
		}

		private async Task SubmitElectricityOrNewPremiseGasMeterReading(SubmitMeterReadingCommand command,
																		AccountInfo accountInfo)
		{
            var historicMeterReadingResult = accountInfo!=null ? (await _queryResolver.GetMeterReadings(accountInfo.AccountNumber, true)).ToArray() : null;
			var sortedMeterResults = MaterReadingDtoSortOrder(command.MeterReadingDataResults);
			MeterReadingResultDto root = null;
	
			if (command.ValidateLastResults)
				ValidateLastHistoryResults(command, accountInfo.ClientAccountType, historicMeterReadingResult);

			foreach (var item in sortedMeterResults)
			{
				var newMeter = BuildMeterReadingResultDto(item);
				if (root == null)
					root = newMeter;
				else
					root.DependentMeterReadingResults.Add(newMeter);
			}

			if (root.DependentMeterReadingResults.Any())
				root.SetAddAsOdata(false);

			try
			{
				await _dataRepository.Add(root);
			} catch (Exception ex)
			{
				var domainException = ex as DomainException;
				if (domainException != null && domainException.DomainError.Equals(ResidentialDomainError.MeterSubmitOutOfTolerance))
				{
					root.MeterReadingNoteID = AllowByPassToleranceCheckNoteID;
					foreach(var dependentMeterReadingResult in root.DependentMeterReadingResults)
					{
						dependentMeterReadingResult.MeterReadingNoteID = AllowByPassToleranceCheckNoteID;
					}						
					await _dataRepository.Add(root);
				}
				else 
				{
					throw;
				}
			}

			if (command.SubmitBusinessActivity)
				await SubmitBusinessActivity(accountInfo);

			MeterReadingCategoryType GetReadingCategory()
			{
				var readingResults = historicMeterReadingResult?.Where(x =>
						x.MeterReadingStatus == MeterReadingStatus.Billed &&
						(x.MeterReadingReason == MeterReadingReason.PeriodicMeterReading ||
						 x.MeterReadingReason == MeterReadingReason.InterimMeterReadingWithBilling ||
						 x.MeterReadingReason == MeterReadingReason.MeterReadingAtMoveOut))
					.OrderByDescending(x => x.ReadingDate).FirstOrDefault();

				return readingResults != null ? readingResults.MeterReadingCategory : MeterReadingCategoryType.Customer;
			}

			MeterReadingResultDto BuildMeterReadingResultDto(MeterReadingData meterReadingData)
			{
				var result = new MeterReadingResultDto();
				var lastHistoricMeterReadingResult = historicMeterReadingResult?.FirstOrDefault(x =>
					x.SerialNumber == meterReadingData.MeterNumber && x.RegisterId == meterReadingData.RegisterId);

				var defaultReadingUnit = meterReadingData.MeterTypeName.Equals(MeterType.Gas.ToString()) ? MeterUnit.M3 : MeterUnit.KWH;

				result.MeterReadingResultID = string.Empty;
				result.DeviceID = meterReadingData.DeviceId;
				result.RegisterID = meterReadingData.RegisterId;
				result.ReadingResult = meterReadingData.MeterReading;
				result.ReadingDateTime = meterReadingData.ReadingDateTime;

				if (!string.IsNullOrEmpty(meterReadingData.MeterReadingReasonID))
				{
					result.MeterReadingReasonID = meterReadingData.MeterReadingReasonID;
				}
				else
				{
					SetMeterReason(result);
				}

				if (command.IsAddGas)
				{
					result.Lcpe = LcpeForAddGas;
					result.FmoRequired = (SapBooleanFlag)meterReadingData.FmoRequired;
				}
				else
				{
					result.Lcpe = meterReadingData.Lcpe ?? lastHistoricMeterReadingResult?.Lcpe ?? string.Empty;
					result.FmoRequired = (SapBooleanFlag)meterReadingData.FmoRequired ?? lastHistoricMeterReadingResult?.FmoRequired ?? string.Empty;
				}

				result.MeterReadingCategoryID = MeterReadingCategoryType.Customer;
				result.SerialNumber = meterReadingData.MeterNumber;
				result.MeterReadingNoteID = ""; //ZI01
				result.Consumption = lastHistoricMeterReadingResult?.Consumption.ToString(CultureInfo.InvariantCulture);
				result.Email = lastHistoricMeterReadingResult?.Email ?? string.Empty;
				result.Vkont = lastHistoricMeterReadingResult?.Vkont ?? string.Empty;
				result.Vertrag = lastHistoricMeterReadingResult?.Vertrag ?? string.Empty;
				result.PchaRequired = lastHistoricMeterReadingResult?.PchaRequired ?? string.Empty;
				result.ReadingUnit = lastHistoricMeterReadingResult?.ReadingUnit ?? defaultReadingUnit;

				result.MeterReadingStatusID = "";
				result.SkipUsrCheck = string.Empty;
				result.MultipleMeterReadingReasonsFlag = null;

				return result;
			}

			void SetMeterReason(MeterReadingResultDto newMeter)
			{
				if (accountInfo.PaymentMethod == PaymentMethodType.Equalizer)
				{
					newMeter.MeterReadingReasonID = MeterReadingReason.InterimMeterReadingWithoutBilling;
				}
				else
				{
					var readingCategory = GetReadingCategory();
					newMeter.MeterReadingReasonID = readingCategory.IsEstimation
						? MeterReadingReason.InterimMeterReadingWithBilling
						: MeterReadingReason.InterimMeterReadingWithoutBilling;
				}

			}
		}

		private void ValidateLastHistoryResults(SubmitMeterReadingCommand command,
												ClientAccountType clientAccountType,
												IEnumerable<MeterReadingInfo> historicMeterReadingResult)
		{
			var rolloverValue = string.IsNullOrWhiteSpace(_meteringSettings.SubmitMeterRolloverValue) ? 0 :
								Convert.ToDecimal(_meteringSettings.SubmitMeterRolloverValue);

			foreach (var meterRead in command.MeterReadingDataResults)
			{
				var readingValue = Convert.ToDecimal(meterRead.MeterReading);

				var lastReadingResult = FilterLastNeworkOrCustomerRecordFromMeterReadingHistory(historicMeterReadingResult,
																								clientAccountType,
																								meterRead.RegisterId,
																								meterRead.MeterNumber,
																								meterRead.MeterTypeName);

				var lastReadingResultValue = lastReadingResult?.Reading ?? 0;
				var isLastReadingNetwork = lastReadingResult?.MeterReadingCategory == MeterReadingCategoryType.Network;

				if (isLastReadingNetwork &&
					lastReadingResultValue > 0 &&
				    (readingValue < lastReadingResultValue) &&
					(lastReadingResultValue - readingValue < rolloverValue))
				{
					Logger.Debug($"ValidateLastHistoryResults rule 1 tiggered validation error");
					throw new DomainException(ResidentialDomainError.MeterReadingLessThanActualNetwork);
				}

				var lastCustomerReadingResult = FilterLastResultFromMeterReadingHistory(historicMeterReadingResult,
																						clientAccountType,
																						meterRead.RegisterId,
																						meterRead.MeterNumber,
																						meterRead.MeterTypeName,
																						MeterReadingCategoryType.Customer);

				var lastCustomerReadingResultValue = lastCustomerReadingResult != null ? lastCustomerReadingResult.Reading : 0;

				if (isLastReadingNetwork &&
					lastReadingResultValue == 0 &&
					(readingValue < lastCustomerReadingResultValue) &&
					(lastCustomerReadingResultValue - readingValue < rolloverValue))
				{
					Logger.Debug($"ValidateLastHistoryResults rule 2 tiggered validation error");
					throw new DomainException(ResidentialDomainError.MeterReadingLessThanActualCustomer);
				}

				var isNetworkReadInHistory = IsNetworkRecordInMeterReadingHistory(historicMeterReadingResult,
																				  clientAccountType,
																				  meterRead.RegisterId,
																				  meterRead.MeterNumber,
																				  meterRead.MeterTypeName);

				if (!isNetworkReadInHistory &&
					(readingValue < lastCustomerReadingResultValue) &&
					(lastCustomerReadingResultValue - readingValue < rolloverValue))
				{
					Logger.Debug($"ValidateLastHistoryResults rule 3 tiggered validation error");
					throw new DomainException(ResidentialDomainError.MeterReadingLessThanActualCustomer);
				}
			}
		}

		private async Task SubmitGasMeterReading(SubmitMeterReadingCommand command, 
												 AccountInfo accountInfo)
		{
			var historicMeterReadingResult = (await _queryResolver.GetMeterReadings(accountInfo.AccountNumber, true)).ToArray();
			if (command.ValidateLastResults)
				ValidateLastHistoryResults(command, accountInfo.ClientAccountType, historicMeterReadingResult);

			var serviceOrder = new ServiceOrderDto
			{
				ServiceOrderID = "",
				ServiceOrderTypeID = ServiceOrderType.Zm71,
				StartDate = DateTimeOffset.UtcNow.Date,
				EndDate = DateTimeOffset.UtcNow.Date,
				ServicePriorityID = "",
				Description = "GAS: Customer Read",
				Note = "",
				ServiceNotificationID = "",
				DeviceID = "",
				AccountID = accountInfo.Partner,
				SystemStatus = "",
				Status = "",
				ConnectionObjectID = accountInfo.PremiseConnectionObjectId,
				CompanyCode = ServiceOrderCompanyCode.SPLY,
				MeterRead = command.MeterReadingDataResults.First().MeterReading,
				WorkCentre = "NETWORKS"
			};
			await _dataRepository.Add(serviceOrder);
		}

		private async Task SubmitBusinessActivity(AccountInfo accountInfo)
		{
			var businessActivityType = PublishBusinessActivityDomainCommand.BusinessActivityType.MeterReading;

			await _commandDispatcher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
				accountInfo.Partner, accountInfo.AccountNumber), true);
		}

		private IEnumerable<MeterReadingData> MaterReadingDtoSortOrder (IEnumerable<MeterReadingData> dataResults)
		{
			var sortOrderList = new List<string>()
			{
			   MeterType.Electricity24h.ToString(),
			   MeterType.ElectricityDay.ToString(),
			   MeterType.ElectricityNight.ToString(),
			   MeterType.Peak.ToString(),
			   MeterType.ElectricityNightStorageHeater.ToString()
			};

			return dataResults.OrderBy(x => sortOrderList.IndexOf(x.MeterTypeName)).ToArray();
		}

		private MeterReadingInfo FilterLastNeworkOrCustomerRecordFromMeterReadingHistory(
					IEnumerable<MeterReadingInfo> meterReadingHistory,
					ClientAccountType accountType,
					string registerId,
					string meterNumber,
					string meterTypeName)
		{
			var minData = DateTime.Today.AddYears(-2);
			
			return accountType == ClientAccountType.Electricity
				? meterReadingHistory?.Where(x => x.RegisterId == registerId &&
												  x.SerialNumber == meterNumber &&
												  x.MeterType.ToString() == meterTypeName &&
												  x.ReadingDate > minData && 
												  x.MeterReadingCategory.IsOneOf(MeterReadingCategoryType.Network, MeterReadingCategoryType.Customer))
					.OrderByDescending(x => x.ReadingDate)
					.FirstOrDefault()
				: meterReadingHistory?.Where(x => x.SerialNumber == meterNumber &&
												  x.ReadingDate > minData &&
												  x.MeterReadingCategory.IsOneOf(MeterReadingCategoryType.Network, MeterReadingCategoryType.Customer))
					.OrderByDescending(x => x.ReadingDate)
					.FirstOrDefault();
		}
		
		private MeterReadingInfo FilterLastResultFromMeterReadingHistory(
					IEnumerable<MeterReadingInfo> meterReadingHistory,
					ClientAccountType accountType,
					string registerId,
					string meterNumber,
					string meterTypeName,
					MeterReadingCategoryType meterReadingCategoryType)
		{
			var minData = DateTime.Today.AddYears(-2);

			return accountType == ClientAccountType.Electricity
				? meterReadingHistory?.Where(x => x.RegisterId == registerId &&
												  x.SerialNumber == meterNumber &&
												  x.MeterType.ToString() == meterTypeName &&
												  x.ReadingDate > minData &&
												  x.MeterReadingCategory == meterReadingCategoryType)
					.OrderByDescending(x => x.ReadingDate)
					.FirstOrDefault()
				: meterReadingHistory?.Where(x => x.SerialNumber == meterNumber &&
												  x.ReadingDate > minData &&
												  x.MeterReadingCategory == meterReadingCategoryType)
					.OrderByDescending(x => x.ReadingDate)
					.FirstOrDefault();
		}

		private bool IsNetworkRecordInMeterReadingHistory(
					IEnumerable<MeterReadingInfo> meterReadingHistory,
					ClientAccountType accountType,
					string registerId,
					string meterNumber,
					string meterTypeName)
		{
			var minData = DateTime.Today.AddYears(-2);

			if (meterReadingHistory == null) return false;

			return accountType == ClientAccountType.Electricity
				? meterReadingHistory.Any(x => x.RegisterId == registerId &&
												  x.SerialNumber == meterNumber &&
												  x.MeterType.ToString() == meterTypeName &&
												  x.ReadingDate > minData &&
												  x.MeterReadingCategory == MeterReadingCategoryType.Network)
				: meterReadingHistory.Any(x => x.SerialNumber == meterNumber &&
												  x.ReadingDate > minData &&
												  x.MeterReadingCategory == MeterReadingCategoryType.Network);
		}
	}
}
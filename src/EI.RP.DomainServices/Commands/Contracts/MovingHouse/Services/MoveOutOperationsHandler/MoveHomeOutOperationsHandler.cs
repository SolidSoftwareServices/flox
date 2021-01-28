using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Commands.Premises.IncommingOccupants;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.CoreServices.Cqrs.Queries;
using System;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveOutOperationsHandler
{
	class MoveHomeOutOperationsHandler : IMoveHomeOutOperationsHandler
	{
		private readonly IDomainCommandDispatcher _commandsDispatcher;
		private readonly IMoveHomeActivityPublisher _activityPublisher;
		private readonly IDomainQueryResolver _queryResolver;

		public MoveHomeOutOperationsHandler(IDomainCommandDispatcher commandsDispatcher, 
											IMoveHomeActivityPublisher activityPublisher,
											IDomainQueryResolver queryResolver)
		{
			_commandsDispatcher = commandsDispatcher;
			_activityPublisher = activityPublisher;
			_queryResolver = queryResolver;
		}

		public async Task SubmitMoveOutMeterReadings(MoveHouse commandData, ContractSaleDto contractSale)
		{			
			var tasks = new List<Task>();
			var electricityMeterReadingDataResults = new List<MeterReadingData>();
			var gasMeterReadingDataResults = new List<MeterReadingData>();

			foreach (var saleDetail in contractSale.SaleDetails)
			{
				var salesProcessTypeID = saleDetail.SalesProcessTypeID;
				var actualMoveOutDate = saleDetail.ActualMoveOutDate;

				var electricitySaleDetailMeterReadings = saleDetail.MeterReadings
											.Where(r => r.DivisionID == DivisionType.Electricity &&
													    r.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut);

				if (electricitySaleDetailMeterReadings.Any())
				{
					electricityMeterReadingDataResults = await AddDataResults(commandData.ElectricityAccount,
																			  salesProcessTypeID,
																			  electricityMeterReadingDataResults,
																			  electricitySaleDetailMeterReadings,
																			  actualMoveOutDate);
				}

				var gasSaleDetailMeterReadings = saleDetail.MeterReadings
										   .Where(r => r.DivisionID == DivisionType.Gas &&
													   r.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut);

				if (gasSaleDetailMeterReadings.Any())
				{
					gasMeterReadingDataResults = await AddDataResults(commandData.GasAccount,
																			  salesProcessTypeID,
																			  gasMeterReadingDataResults,
																			  gasSaleDetailMeterReadings,
																			  actualMoveOutDate);
				}

			}

			if (electricityMeterReadingDataResults.Any())
			{
				var cmd = new SubmitMeterReadingCommand(commandData.ElectricityAccount, 
														electricityMeterReadingDataResults,
														submitBusinessActivity: false);
				tasks.Add(ExecuteCommand(cmd));
			}

			if (gasMeterReadingDataResults.Any())
			{
				var cmd = new SubmitMeterReadingCommand(commandData.GasAccount, 
														gasMeterReadingDataResults,
														submitBusinessActivity: false);
				tasks.Add(ExecuteCommand(cmd));
			}

			await Task.WhenAll(tasks.ToArray());
			commandData.Context.CheckPoints.SubmitMoveOutMeterReadCompleted_2 = true;

			async Task ExecuteCommand(SubmitMeterReadingCommand cmd)
			{
				try
				{
					await _commandsDispatcher.ExecuteAsync(cmd, true);
				}
				catch (DomainException ex)
				{
					//second attempt
					if (!ex.DomainError.Equals(ResidentialDomainError.DataAlreadyReleased) &&
						!ex.DomainError.Equals(ResidentialDomainError.MeterReadingAlreadyReceived))
					{
						await _activityPublisher.SubmitActivityError(commandData, ex);
					}
					throw;
				}
			}
		}

		private async Task<List<MeterReadingData>> AddDataResults(string accountNumber,
																  string salesProcessTypeID,
																  List<MeterReadingData> meterReadingDataResults, 
																  IEnumerable<ContractSaleMeterReadingDto> saleDetailMeterReadings,
																  DateTime? actualMoveOutDate)
		{
			var devices = await _queryResolver.GetDevicesByAccount(accountNumber,byPassPipeline: true);
			var registers = devices.SelectMany(x => x.Registers);

			foreach (var meterReaderResult in saleDetailMeterReadings)
			{
				var registerMeterType = registers.Where(x => x.MeterNumber == meterReaderResult.SerialNumber &&
															 x.RegisterId == meterReaderResult.RegisterID)
												 .Select(x => x.MeterType).First();
			
				var meterReadingData = new MeterReadingData
				{
					DeviceId = meterReaderResult.DeviceID,
					MeterReading = meterReaderResult.DivisionID == DivisionType.Electricity ? 
									meterReaderResult.ReadingResult : await ResolveMoveOutReadingReadingResultForGas(meterReaderResult, accountNumber),
					RegisterId = meterReaderResult.RegisterID,
					MeterNumber = meterReaderResult.SerialNumber,
					MeterTypeName = registerMeterType.ToString(),
					Lcpe = salesProcessTypeID,
					ReadingDateTime = meterReaderResult.DivisionID == DivisionType.Electricity ?
										meterReaderResult.ReadingDateTime : actualMoveOutDate.GetValueOrDefault(),
					MeterReadingReasonID = meterReaderResult.MeterReadingReasonID
				};
				meterReadingDataResults.Add(meterReadingData);
			}

			return meterReadingDataResults;
		}

		public async Task StoreIncommingOccupant(MoveHouse commandData)
		{
			var moveOutDetails = commandData.Context.MoveOutDetails;
			if (moveOutDetails.IncomingOccupant)
				await Task.WhenAll(_commandsDispatcher.ExecuteAsync(
						new NotifyNewIncommingOccupant(commandData.Context.Electricity?.Account.AccountNumber,
							moveOutDetails.LettingAgentName,
							moveOutDetails.LettingPhoneNumber)
						, true),
					commandData.Context.Gas?.Account != null
						? _commandsDispatcher.ExecuteAsync(
							new NotifyNewIncommingOccupant(commandData.Context.Gas?.Account.AccountNumber,
								moveOutDetails.LettingAgentName,
								moveOutDetails.LettingPhoneNumber)
							, true)
						: Task.CompletedTask);

			commandData.Context.CheckPoints.StoreNewIncommingOccupantCompleted_1 = true;
		}

		private async Task<string> ResolveMoveOutReadingReadingResultForGas(ContractSaleMeterReadingDto meterReading, string accountNumber)
		{
			var ret = meterReading.ReadingResult;
			var meterReadingHistoryTask = _queryResolver.GetMeterReadings(accountNumber);
			var meterReadingHistory = await meterReadingHistoryTask;

			var lastMeterReadInfo = meterReadingHistory?.Where(x => x.SerialNumber == meterReading.SerialNumber &&
																		(x.MeterReadingCategory == MeterReadingCategoryType.Network ||
																		x.MeterReadingCategory == MeterReadingCategoryType.Customer))
															  .OrderByDescending(x => x.ReadingDate)
															  .FirstOrDefault();

			if (lastMeterReadInfo != null)
			{
				var lastMeterReadResult = lastMeterReadInfo.Reading;
				if (Convert.ToDecimal(ret) < lastMeterReadResult)
				{
					ret = (lastMeterReadResult + 1).ToString();
				}
			}

			return ret;
		}
	}
}
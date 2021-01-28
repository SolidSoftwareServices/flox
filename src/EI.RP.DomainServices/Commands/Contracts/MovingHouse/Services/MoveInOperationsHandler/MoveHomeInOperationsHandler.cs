using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveInOperationsHandler
{
	class MoveHomeInOperationsHandler : IMoveHomeInOperationsHandler
	{
		private readonly IDomainCommandDispatcher _commandsDispatcher;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IMoveHomeActivityPublisher _activityPublisher;

		public MoveHomeInOperationsHandler(IDomainCommandDispatcher commandsDispatcher,
										   IDomainQueryResolver queryResolver, 
										   IMoveHomeActivityPublisher activityPublisher)
		{
			_commandsDispatcher = commandsDispatcher;
			_queryResolver = queryResolver;
			_activityPublisher = activityPublisher;
		}

		public async Task SubmitMoveInMeterReadings(MoveHouse commandData, ContractSaleDto contractSale)
		{
			var tasks = new List<Task>();
			var electricityMeterReadingDataResults = new List<MeterReadingData>();
			var gasMeterReadingDataResults = new List<MeterReadingData>();

			foreach (var saleDetail in contractSale.SaleDetails)
			{
				var prn = ResolvePointReferenceNumber(commandData, saleDetail);
				if (prn == null) continue;

				var pointOfDeliveryInfo = await _queryResolver.GetPointOfDeliveryInfoByPrn(prn, true);
				var salesProcessTypeID = saleDetail.SalesProcessTypeID;
				var isNewAcquisition = await _queryResolver.IsPrnNewAcquisition(prn, pointOfDeliveryInfo.IsNewAcquisition);

				if (!isNewAcquisition)
				{
					var electricitySaleDetailMeterReadings = saleDetail.MeterReadings
								.Where(r => r.DivisionID == DivisionType.Electricity &&
											r.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveIn);

					if (electricitySaleDetailMeterReadings.Any())
					{						
						electricityMeterReadingDataResults = await AddDataResults(salesProcessTypeID,
																				  electricityMeterReadingDataResults,
																				  electricitySaleDetailMeterReadings,
																				  commandData.Context.NewPrns.NewMprn);
					}

					var gasSaleDetailMeterReadings = saleDetail.MeterReadings
											   .Where(r => r.DivisionID == DivisionType.Gas &&
														   r.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveIn);

					if (gasSaleDetailMeterReadings.Any())
					{
						gasMeterReadingDataResults = await AddDataResults(salesProcessTypeID,
																		  gasMeterReadingDataResults,
																		  gasSaleDetailMeterReadings,
																		  commandData.Context.NewPrns.NewGprn);
					}
				}
			}

			if (electricityMeterReadingDataResults.Any())
			{
				var cmd = new SubmitMeterReadingCommand(commandData.ElectricityAccount,
														electricityMeterReadingDataResults,
														newPremisePrn: commandData.Context.NewPrns.NewMprn,
														submitBusinessActivity: false);
				tasks.Add(ExecuteCommand(cmd));
			}

			if (gasMeterReadingDataResults.Any())
			{
				var cmd = new SubmitMeterReadingCommand(commandData.GasAccount,
														gasMeterReadingDataResults,
														newPremisePrn: commandData.Context.NewPrns.NewGprn,
														submitBusinessActivity: false);
				tasks.Add(ExecuteCommand(cmd));
			}

			await Task.WhenAll(tasks.ToArray());
			commandData.Context.CheckPoints.SubmitMoveInMeterReadCompleted_3 = true;
			
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

		private async Task<List<MeterReadingData>> AddDataResults(string salesProcessTypeID,
																  List<MeterReadingData> meterReadingDataResults,
																  IEnumerable<ContractSaleMeterReadingDto> saleDetailMeterReadings,
																  PointReferenceNumber newPrn)
		{
			var newPremise = await _queryResolver.GetPremiseByPrn(newPrn);

			foreach (var meterReaderResult in saleDetailMeterReadings)
			{
				var deviceInfo = await _queryResolver.GetNewPremiseDeviceById(newPrn, meterReaderResult.DeviceID, true);
				var registers = deviceInfo.Registers;

				var registerMeterType = registers.Where(x => x.MeterNumber == meterReaderResult.SerialNumber &&
															 x.RegisterId == meterReaderResult.RegisterID)
												 .Select(x => x.MeterType).First();

				var fmoRequired = await ResolveFmoRequired(meterReaderResult, newPremise);
				var dateAndResultValue = meterReaderResult.DivisionID == DivisionType.Gas ? ResolveMoveInReadingDateTimeResultForGas(meterReaderResult, newPremise) : 
												new DateAndResultValue(meterReaderResult.ReadingDateTime, meterReaderResult.ReadingResult);

				var meterReadingData = new MeterReadingData
				{
					DeviceId = meterReaderResult.DeviceID,
					MeterReading = dateAndResultValue.ReadingResult,
					RegisterId = meterReaderResult.RegisterID,
					MeterNumber = meterReaderResult.SerialNumber,
					MeterTypeName = registerMeterType.ToString(),
					Lcpe = salesProcessTypeID,
					ReadingDateTime = dateAndResultValue.ReadingDateTime,
					MeterReadingReasonID = meterReaderResult.MeterReadingReasonID,
					FmoRequired = fmoRequired
				};
				meterReadingDataResults.Add(meterReadingData);
			}

			return meterReadingDataResults;
		}

		private async Task<bool> ResolveFmoRequired(ContractSaleMeterReadingDto meterReading, Premise newPremise)
		{
			var fmoRequired = false;
			var device = newPremise.Installations.SelectMany(x=> x.Devices).Single(x=> x.DeviceId == meterReading.DeviceID);
			var meterReadMoveIn = device.MeterReadingResults.LastOrDefault(x =>
				x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveIn);
			var meterReadMoveOut = device.MeterReadingResults.LastOrDefault(x =>
				x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut);

			if (device.MeterReadingResults.Last().MeterReadingReasonID ==
			    MeterReadingReason.MeterReadingAtBillingRelInst)
			{
				throw new DomainException(ResidentialDomainError.CantProcessMoveInMoveOut,
					$"Last meter reading moving in  reason MeterReadingAtBillingRelInst.");
			}

			if (meterReadMoveIn == null && meterReadMoveOut == null
			    || meterReadMoveOut == null
			    || !(meterReadMoveIn == null || meterReadMoveOut.ReadingDateTime > meterReadMoveIn.ReadingDateTime))
			{
				fmoRequired = true;
			}

			return fmoRequired;
		}

		private static PointReferenceNumber ResolvePointReferenceNumber(MoveHouse commandData,
			ContractSaleDetailDto saleDetail)
		{
			PointReferenceNumber prn = null;
			if (saleDetail.DivisionID == DivisionType.Gas)
			{
				prn = commandData.Context.NewPrns.NewGprn;
			}
			else if (saleDetail.DivisionID == DivisionType.Electricity)
			{
				prn = commandData.Context.NewPrns.NewMprn;
			}

			return prn;
		}

		private DateAndResultValue ResolveMoveInReadingDateTimeResultForGas(ContractSaleMeterReadingDto meterReading, Premise newPremise)
		{
			var ret = new DateAndResultValue(meterReading.ReadingDateTime, meterReading.ReadingResult);
			var device = newPremise?.Installations.SelectMany(x => x.Devices).Single(x => x.DeviceId == meterReading.DeviceID);
			var lastMeterReadInfo = device?.MeterReadingResults.Where(x => x.SerialNumber == meterReading.SerialNumber &&
																		(x.MeterReadingCategory == MeterReadingCategoryType.Network ||
																		x.MeterReadingCategory == MeterReadingCategoryType.Customer))
															  .OrderByDescending(x=>x.ReadingDate)
															  .FirstOrDefault();

			if (lastMeterReadInfo!=null)
			{
				var lastMeterReadDate = lastMeterReadInfo.ReadingDate.GetValueOrDefault();
				if (ret.ReadingDateTime < lastMeterReadDate)
				{
					ret.ReadingDateTime = DateTime.Today;
				}

				var lastMeterReadResult = lastMeterReadInfo.Reading;
				if (Convert.ToDecimal(ret.ReadingResult) < lastMeterReadResult)
				{
					ret.ReadingResult = (lastMeterReadResult + 1).ToString();
				}
			}

			return ret;
		}

		private class DateAndResultValue
		{
			public DateTime ReadingDateTime { get; set; }
			public string ReadingResult { get; set; }

			public DateAndResultValue(DateTime readingDateTime, string readingResult)
			{
				ReadingDateTime = readingDateTime;
				ReadingResult = readingResult;
			}
		} 
	}
}
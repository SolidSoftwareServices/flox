using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.Queries.Metering.Premises;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainErrors;
using EI.RP.CoreServices.ErrorHandling;

namespace EI.RP.DomainServices.InternalShared.MeterReading
{
	internal class SubmitMeterReadings : ISubmitMeterReadings
	{
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IDomainQueryResolver _queryResolver;

		public SubmitMeterReadings(IDomainCommandDispatcher commandDispatcher, IDomainQueryResolver queryResolver)
		{
			_commandDispatcher = commandDispatcher;
			_queryResolver = queryResolver;
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

		public async Task SubmitGasMeterReading(string accountNumber, bool isNewAcquisition,
			GasPointReferenceNumber premisePrn, decimal meterReading, bool isAddGas = false)
		{
			if (!isNewAcquisition)
			{
				var premise = await _queryResolver.GetPremiseByPrn(premisePrn, true);
				var device = premise.Installations.SelectMany(x => x.Devices)
					.SingleOrDefault(x => x.MeterType.IsGas());

				var meterReadingDataResults = new List<MeterReadingData>();
				var meterReadingData = new MeterReadingData
				{
					DeviceId = device.DeviceId,
					MeterReading = meterReading.ToString(),
					RegisterId = device.Registers.First().RegisterId,
					MeterNumber = device.Registers.First().MeterNumber,
					MeterTypeName = device.Registers.First().MeterType.ToString(),
				};

				meterReadingData = isAddGas ? SetFmoRequired(deviceInfo: device, meterReadingData) : meterReadingData;

				meterReadingDataResults.Add(meterReadingData);

				var cmd = new SubmitMeterReadingCommand(accountNumber, meterReadingDataResults, newPremisePrn: premisePrn, submitBusinessActivity: !isAddGas, isAddGas: isAddGas);
				await _commandDispatcher.ExecuteAsync(cmd);
			}
		}		
	}
}
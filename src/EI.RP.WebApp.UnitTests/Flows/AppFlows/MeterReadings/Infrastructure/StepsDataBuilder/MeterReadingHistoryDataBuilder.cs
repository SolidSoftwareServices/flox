using System;
using System.Collections.Generic;
using AutoFixture;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MeterReadings.Infrastructure.StepsDataBuilder
{
	class MeterReadingHistoryDataBuilder
	{
		readonly IFixture _fixture = new Fixture().CustomizeDomainTypeBuilders();

		public IEnumerable<MeterReadingInfo> CreateMeterReadingHistory(ClientAccountType accountType, 
																	   SubmitMeterReading.ScreenModel.MeterData[] meterReadings, 
																	   MeterReadingCategoryType meterReadingCategory,
																	   bool lowerThanMeterReadings = true)
		{			
			var meterReadingHistoryResults = new List<MeterReadingInfo>();

			foreach (var meterReading in meterReadings)
			{
				var meterReadingValue = Convert.ToDecimal(meterReading.ReadingValue);
				var meterReadingHistory = _fixture.Build<MeterReadingInfo>()
					.With(x => x.AccountType, accountType)
					.With(x => x.MeterType, MeterType.From(meterReading.MeterTypeName))
					.With(x => x.DeviceId, meterReading.DeviceId)
					.With(x => x.RegisterId, meterReading.RegisterId)
					.With(x => x.SerialNumber, meterReading.MeterNumber)
					.With(x => x.MeterReadingCategory, meterReadingCategory)
					.With(x => x.Reading, lowerThanMeterReadings ? meterReadingValue - 1 : meterReadingValue + 1)
					.Create();

				meterReadingHistoryResults.Add(meterReadingHistory);
			}

			return meterReadingHistoryResults;
		}
	}
}
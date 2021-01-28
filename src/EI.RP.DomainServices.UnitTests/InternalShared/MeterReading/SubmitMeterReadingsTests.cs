using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.InternalShared.MeterReading;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using NUnit.Framework;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;

namespace EI.RP.DomainServices.UnitTests.InternalShared.MeterReading
{
	internal class SubmitMeterReadingsTests : UnitTestFixture<SubmitMeterReadingsTests.TestContext, SubmitMeterReadings>
	{
		public class TestContext : UnitTestContext<SubmitMeterReadings>
		{
			private string _accountNumber;
			private GasPointReferenceNumber _gprn;
			private decimal _meterReading;

			public Premise Premise { get; set; }
			public DeviceInfo DeviceInfo { get; private set; }

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountNumber(string accountNumber)
			{
				_accountNumber = accountNumber;
				return this;
			}

			public TestContext WithGPRN(GasPointReferenceNumber gprn)
			{
				_gprn = gprn;
				return this;
			}

			public TestContext WithMeterReading(decimal meterReading)
			{
				_meterReading = meterReading;
				return this;
			}

			public void SetupMocks()
			{
				DomainFacade.SetUpMocker(AutoMocker);

				DeviceInfo = Fixture
					.Build<DeviceInfo>()
					.With(d => d.MeterType, MeterType.Gas)
					.With(d => d.MeterReadingResults, new List<MeterReadingInfo>()
					{
					   new MeterReadingInfo { MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveIn },
					   new MeterReadingInfo { MeterReadingReasonID = MeterReadingReason.MeterReadingAtMoveOut },
					})
					.Create();

				var installations = Fixture.Build<InstallationInfo>()
					.With(x => x.Devices, DeviceInfo.ToOneItemArray())
					.Create()
					.ToOneItemArray()
					.AsEnumerable();

				Premise = Fixture.Build<Premise>()
					.With(x => x.Installations, installations)
					.Create();

				DomainFacade.QueryResolver.ExpectQuery(new PremisesQuery
				{
					Prn = _gprn
				}, Premise.ToOneItemArray().AsEnumerable());
			}
		}

		private MeterReadingData SetFmoRequired(DeviceInfo deviceInfo, MeterReadingData meterReadingData)
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

		[Test]
		[Theory]
		public async Task CanExecute(bool isNewAcquisition, bool isAddGas)
		{
			Context.Fixture.CustomizeDomainTypeBuilders();
			//input
			var gprn = Context.Fixture.Create<GasPointReferenceNumber>();
			var electricityAccountNumber = Context.Fixture.Create<long>().ToString();
			var meterReading = Context.Fixture.Create<decimal>();
			var meterReadingResults = Context.Fixture.CreateMany<MeterReadingData>(1).ToList();
			SubmitMeterReadingCommand cmd = null;

			Context
				.WithAccountNumber(electricityAccountNumber)
				.WithGPRN(gprn)
				.WithMeterReading(meterReading)
				.SetupMocks();

			if (!isNewAcquisition)
			{
				var device = Context.DeviceInfo;

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
				cmd = new SubmitMeterReadingCommand(electricityAccountNumber, meterReadingDataResults, newPremisePrn: gprn, submitBusinessActivity: !isAddGas, isAddGas: isAddGas);
				Context.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);			
			}
			
			await Context
				.Sut
				.SubmitGasMeterReading(electricityAccountNumber, isNewAcquisition, gprn, meterReading, isAddGas);

			if (isNewAcquisition)
			{
				Context.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SubmitMeterReadingCommand>();				
			}
			else
			{
				Context.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);				
			}
		}		
	}
}

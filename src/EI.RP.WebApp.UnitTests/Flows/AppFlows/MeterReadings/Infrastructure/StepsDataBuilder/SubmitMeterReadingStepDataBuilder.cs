using System.Linq;
using System.Collections.Generic;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MeterReadings.Infrastructure.StepsDataBuilder
{
	class SubmitMeterReadingStepDataBuilder
	{
		private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;
		IFixture _fixture = new Fixture().CustomizeDomainTypeBuilders();

		public SubmitMeterReadingStepDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			//if need to support more than one do it explicitly
			_accountConfigurator = appUserConfigurator.ElectricityAndGasAccountConfigurators.Single();
		}

		public SubmitMeterReading.ScreenModel Create()
		{
			var builder = _accountConfigurator.DomainFacade.ModelsBuilder;
			var devices = _accountConfigurator.Premise.Devices.ToArray();
			var deviceRegisterInfos = devices.SelectMany(x => x.Registers)
											 .ToArray();

			var accountNumber = _accountConfigurator.Model.AccountNumber;

			var composer = builder
				.Build<SubmitMeterReading.ScreenModel>()
				.With(x => x.AccountType, _accountConfigurator.AccountType)
				.With(x => x.AccountNumber, accountNumber)
				.With(x => x.MeterReadings, BuildMeterReadings(deviceRegisterInfos));

			return composer.Create();

			SubmitMeterReading.ScreenModel.MeterData[] BuildMeterReadings(DeviceRegisterInfo[] registerInfos)
			{
				
				var meterReadings = new List<SubmitMeterReading.ScreenModel.MeterData>();
				foreach (var registerInfo in registerInfos)
				{
					var meterReading = new SubmitMeterReading.ScreenModel.MeterData
					{
						DeviceId = registerInfo.DeviceId,
						MeterNumber = registerInfo.MeterNumber,
						ReadingValue = _fixture.Create<int>().ToString(),
						MeterUnit = registerInfo.MeterUnit,
						MeterTypeName = registerInfo.MeterType.ToString(),
						RegisterId = registerInfo.RegisterId
					};
					meterReadings.Add(meterReading);
				}
				return meterReadings.ToArray();
			}
		}
	}
}
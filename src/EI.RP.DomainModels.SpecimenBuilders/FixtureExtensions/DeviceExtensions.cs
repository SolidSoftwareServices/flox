using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;

namespace EI.RP.DomainModels.SpecimenBuilders.FixtureExtensions
{
	//TODO: MOVE TO THE SPECIMES BUILDER
	public static class DeviceExtensions
	{
		private static int _deviceId=1000;

		public static DeviceInfo CreateDevice(this IFixture fixture, string contractId, params MeterType[] meterTypes)
		{
			return fixture.CreateDevice(contractId, false, meterTypes);
		}

		public static DeviceInfo CreateDevice(this IFixture fixture, string contractId, bool isSmart, params MeterType[] meterTypes)
		{
			return fixture.CreateDevice(contractId, false, CommsTechnicallyFeasibleValue.NotAvailableYet, RegisterConfigType.MCC01,  meterTypes);
		}

		public static DeviceInfo CreateDevice(this IFixture fixture, string contractId, bool isSmart, CommsTechnicallyFeasibleValue ctf,RegisterConfigType configType,
			params MeterType[] meterTypes)
		{
			var deviceId = Interlocked.Increment(ref _deviceId).ToString();
			var registerInfos = new DeviceRegisterInfo[0];

			if (meterTypes.Any() && configType != RegisterConfigType.MCC12)
			{
				registerInfos = fixture.Build<DeviceRegisterInfo>()
					.With(x => x.RegisterId, MeterReadingRegisterType.ActiveEnergyRegisterType)
					.With(x => x.DeviceId, deviceId)

					.CreateMany(meterTypes.Length).Select((x, i) =>
					{
						x.MeterType = meterTypes[i];
						x.MeterNumber = Interlocked.Increment(ref _deviceId).ToString();
						x.MeterUnit = x.MeterType == MeterType.Gas ? MeterUnit.M3 : MeterUnit.KWH;
						return x;
					}).ToArray();

			}

			return fixture.Build<DeviceInfo>()
				.With(x=> x.DeviceId, deviceId)
				.With(x=> x.DivisionId,meterTypes.Any()?
					meterTypes.First().IsElectricity()?DivisionType.Electricity:DivisionType.Gas
					:DivisionType.Electricity)
				.With(x=> x.ContractId, contractId)
				.With(x=> x.Registers, registerInfos)
				.With(x=> x.MeterType, meterTypes.First)
				.With(x=> x.MCCConfiguration,configType)
				.With(x=> x.CTF, ctf)
				.With(x=> x.IsSmart,isSmart)
				.With(x=>x.SmartActivationStatus, ResolveSmartActivationStatus())
				.Create();

			SmartActivationStatus ResolveSmartActivationStatus()
			{
				var smartActivationStatus = SmartActivationStatus.SmartNotAvailable;
				if (isSmart && 
				    (meterTypes.Any() ? meterTypes.First().IsElectricity() ? DivisionType.Electricity : DivisionType.Gas : DivisionType.Electricity) == DivisionType.Electricity)
				{
					if (configType != null && configType.IsSmartConfigurationActive())
					{
						smartActivationStatus = SmartActivationStatus.SmartActive;
					}
					else if (configType != null && configType.CanOptToSmartActive())
					{
						smartActivationStatus = ctf.AllowsAllSmartFeatures() ? SmartActivationStatus.SmartAndEligible : SmartActivationStatus.SmartButNotEligible;
					}
				}

				return smartActivationStatus;
			}
		}

		public static IEnumerable<DeviceInfo> CreateDevices(this IFixture fixture, bool smart,
			ConfigurableDeviceSet deviceSet)
		{
			return fixture.CreateDevices(fixture.Create<string>(), smart, deviceSet);
		}

		public static IEnumerable<DeviceInfo> CreateDevices(this IFixture fixture, string contractId, bool isSmart,
			ConfigurableDeviceSet deviceSet)
		{
			return fixture.CreateDevices(contractId, isSmart, deviceSet, CommsTechnicallyFeasibleValue.NotAvailableYet,
				RegisterConfigType.MCC01);
		}
		public static IEnumerable<DeviceInfo> CreateDevices(this IFixture fixture, string contractId, bool isSmart, ConfigurableDeviceSet deviceSet,CommsTechnicallyFeasibleValue ctf, RegisterConfigType configType)
		{
			var result=new List<DeviceInfo>();
			switch (deviceSet)
			{

				case ConfigurableDeviceSet.Gas:
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.Gas));
					break;
				case ConfigurableDeviceSet.Electricity24H:
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.Electricity24h));
					break;
				case ConfigurableDeviceSet.ElectricityDayAndNightMeters:
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.ElectricityDay, MeterType.ElectricityNight));
					break;
				case ConfigurableDeviceSet.ElectricityNightStorageHeater:
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.ElectricityNightStorageHeater));
					break;
				case ConfigurableDeviceSet.Electricity24HAndNightStorageHeater:
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.ElectricityNightStorageHeater));
					result.Add(fixture.CreateDevice(contractId, isSmart, MeterType.Electricity24h));
					break;				
				
			}

			return result.ToArray();
		}
	}


}
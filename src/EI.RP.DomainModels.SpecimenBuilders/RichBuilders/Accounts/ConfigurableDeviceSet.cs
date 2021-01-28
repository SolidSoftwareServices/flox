using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	public enum ConfigurableDeviceSet
	{
		None = 0,
		Gas,
		Electricity24H,
		ElectricityDayAndNightMeters,
		Electricity24HAndNightStorageHeater,
		ElectricityNightStorageHeater
	}

	public class ConfigurableDeviceInfo
	{
		public ConfigurableDeviceInfo()
		{
		}

		public ConfigurableDeviceInfo(ConfigurableDeviceSet deviceType, RegisterConfigType registerConfigType,
			CommsTechnicallyFeasibleValue ctf)
		{
			DeviceType = deviceType;
			RegisterConfigType = registerConfigType;
			CTF = ctf;
			IsSmart = registerConfigType != null && ctf != null && (registerConfigType == RegisterConfigType.MCC12 || (registerConfigType.CanOptToSmartActive() &&
			          ctf.AllowsAllSmartFeatures()));
		}

		public bool IsSmart { get; }

		public ConfigurableDeviceSet DeviceType { get;set; }
		public RegisterConfigType RegisterConfigType { get; set; }
		public CommsTechnicallyFeasibleValue CTF{ get; set; }
	}
}
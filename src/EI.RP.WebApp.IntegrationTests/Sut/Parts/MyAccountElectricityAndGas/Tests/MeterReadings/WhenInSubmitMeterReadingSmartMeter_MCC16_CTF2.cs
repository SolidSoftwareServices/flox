using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	internal class WhenInSubmitMeterReadingSmartMeter_MCC16_CTF2 : WhenInSubmitMeterReadingSmartMeter
	{
		protected override CommsTechnicallyFeasibleValue CTF => CommsTechnicallyFeasibleValue.CTF2;
		protected override RegisterConfigType MccConfiguration =>RegisterConfigType.MCC16;
		protected override bool HasMeterHistoryAvailable => true;
	}
}

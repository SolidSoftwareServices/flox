using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MeterReadings
{
	internal class WhenInSubmitMeterReadingSmartMeter_MCC01_CTF3 : WhenInSubmitMeterReadingSmartMeter
	{
		protected override CommsTechnicallyFeasibleValue CTF => CommsTechnicallyFeasibleValue.CTF3;
		protected override RegisterConfigType MccConfiguration =>RegisterConfigType.MCC01;
		protected override bool HasMeterHistoryAvailable => true;
	}
}

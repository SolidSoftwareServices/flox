using System.Collections.Generic;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts
{
	internal interface IEnvironmentInputs
	{
		IDictionary<string, string> ContactUs { get; }
		IDictionary<string, string> Usage { get; }
		IDictionary<string, string> RegisterAccount { get; }
		IDictionary<string, string> EnergyServices { get; }
		IDictionary<string, string> MeterReading { get; }
		IDictionary<string, string> AddGas { get; }
		IDictionary<string, string> EditDd { get; }
		IDictionary<string, string> MakeAPayment { get; }
		IDictionary<string, string> AgentLogin { get; }
		IDictionary<string, string> Refund { get; }
		IDictionary<string, string> MoveHouse { get; }
		IDictionary<string, string> GasEqualizer { get; }
		IDictionary<string, string> EqualizerEligable { get; }
		IDictionary<string, string> ContactUsAddAccount { get; }
		IDictionary<string, string> AdminSearch { get; }
		IDictionary<string, string> MeterReadingElec { get; }
		IDictionary<string, string> MeterReadingDayNight { get; }
		IDictionary<string, string> MeterReadingGas { get; }
	}
}
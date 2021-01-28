using System.Collections.Generic;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts
{
	static partial class EnvironmentSet
	{
		private class TestInputs : IEnvironmentInputs
		{
			public IDictionary<string, string> account { get; } = new Dictionary<string, string>()
			{
				{"Email", "testAuto3@esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900021411"}
			};

			public IDictionary<string, string> ContactUs => throw new System.NotImplementedException();

			public IDictionary<string, string> Usage => throw new System.NotImplementedException();

			public IDictionary<string, string> RegisterAccount => throw new System.NotImplementedException();

			public IDictionary<string, string> EnergyServices { get; } = new Dictionary<string, string>()
			{
				{"Email", "TESTESA@gmail.com"},
				{"Password", "Test3333"},
				{"Account", "950747755"}
			};

			public IDictionary<string, string> MeterReading { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"},
			};

			public IDictionary<string, string> AddGas { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"},
			};

			public IDictionary<string, string> EditDd { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"},
			};

			public IDictionary<string, string> MakeAPayment { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"}
			};

			public IDictionary<string, string> AgentLogin { get; } = new Dictionary<string, string>()
			{
				{"Email", "tilley_j"},
				{"Password", "Init1234"}
			};

			public IDictionary<string, string> Refund { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR851_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "901676061"}
			};

			public IDictionary<string, string> MoveHouse { get; } = new Dictionary<string, string>()
			{
				{"Email", "testAuto3@esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900021411"}
			};

			public IDictionary<string, string> GasEqualizer => throw new System.NotImplementedException();

			public IDictionary<string, string> EqualizerEligable => throw new System.NotImplementedException();

			public IDictionary<string, string> ContactUsAddAccount => throw new System.NotImplementedException();

			public IDictionary<string, string> AdminSearch { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecOnly1@esb.ie"},
				{"Password", "Test3333"}
			};

			#region MeterReading

			public IDictionary<string, string> MeterReadingElec { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"},
			};

			public IDictionary<string, string> MeterReadingDayNight { get; } = new Dictionary<string, string>()
			{
				{"Email", "daynight3@esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900285076"},
				{"Type", "Day and night"}
			};

			public IDictionary<string, string> MeterReadingGas { get; } = new Dictionary<string, string>()
			{
				{"Email", "gpayg@esb.ie"},
				{"Password", "Test3333"},
				{"Account", "903807665"},
			};

			#endregion
		}
	}
}
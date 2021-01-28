using System.Collections.Generic;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts
{
	static partial class EnvironmentSet
	{
		private class PreProdInputs : IEnvironmentInputs
		{
			public IDictionary<string, string> ContactUs { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR127_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900674099"}
			};

			public IDictionary<string, string> Usage { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR128_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900070856"}
			};

			public IDictionary<string, string> RegisterAccount { get; } = new Dictionary<string, string>()
			{
				{"Email", "joebloggs@gmail.com"},
				{"FirstName", "Joe"},
				{"LastName", "Bloggs"},
				{"AccountNumber", "903537019"},
				{"MPRN", "958903"},
				{"DOB", "10/09/1992"},
				{"Phone", "0868613572"}
			};

			public IDictionary<string, string> EnergyServices { get; } = new Dictionary<string, string>()
			{
				{"Email", "esa_test@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "950799029"}
			};

			public IDictionary<string, string> MeterReading { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR123_ELEC-1METER@PREPROD.ESB.IE"},
				{"Password", "Test3333"},
				{"Account", "901666774"}
			};

			public IDictionary<string, string> AddGas { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"}
			};

			public IDictionary<string, string> EditDd { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900294377"}
			};

			public IDictionary<string, string> MakeAPayment { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR124_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "901245787"}
			};

			public IDictionary<string, string> AgentLogin { get; } = new Dictionary<string, string>()
			{
				{"Email", "leahy_de"},
				{"Password", "Welcome100"}
			};

			public IDictionary<string, string> Refund { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR851_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "901676061"}
			};

			public IDictionary<string, string> MoveHouse { get; } = new Dictionary<string, string>()
			{
				{"Email", "elecdd33@esb.ie"},
				{"Password", "Test9999"},
				{"Account", "900039891"}
			};

			public IDictionary<string, string> GasEqualizer { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR503_Gas-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "903799787"}
			};

			public IDictionary<string, string> EqualizerEligable { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR135_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "901520911"}
			};

			public IDictionary<string, string> ContactUsAddAccount { get; } = new Dictionary<string, string>()
			{
				{"Account", "950492961"},
				{"MPRN", "755596"}
			};

			public IDictionary<string, string> AdminSearch { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR125_Elec-1Meter@preprod.esb.ie"},
				{"BP", "1000027631"},
				{"Street", "Sandymount"},
				{"House", "1"},
				{"City", "Dublin"},
				{"MaximumRecords", "30"},
			};


			#region MeterReading

			public IDictionary<string, string> MeterReadingElec { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR395_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "900737656"},
				{"Mprn", "10000437266"}
			};

			public IDictionary<string, string> MeterReadingDayNight { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR3014_Elec-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "904645671"},
				{"Type", "Day and night"},
				{"Mprn", "10001349588"}
			};

			public IDictionary<string, string> MeterReadingGas { get; } = new Dictionary<string, string>()
			{
				{"Email", "MR2556_Gas-1Meter@preprod.esb.ie"},
				{"Password", "Test3333"},
				{"Account", "903798949"},
				{"Mprn", "1075887"}
			};

			#endregion
		}
	}
}
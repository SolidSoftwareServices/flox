using System.ComponentModel.DataAnnotations;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public class InputFields
	{
		public string Mprn { get; set; }
		public string Gprn { get; set; }
		public string MeterReadingDescription { get; set; }
		public bool Electricity24HrsDevicesFieldRequired { get; set; }
		public bool ElectricityDayDevicesFieldRequired { get; set; }
		public bool ElectricityNightDevicesFieldRequired { get; set; }
		public bool ElectricityNightStorageHeaterDevicesFieldRequired { get; set; }
		public bool GasDevicesFieldRequired { get; set; }

        [RequiredIf(nameof(Electricity24HrsDevicesFieldRequired), IfValue = true, ErrorMessage = "Please enter a valid meter reading")]
		[RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading")]
		public string MeterReading24Hrs { get; set; }
		[RequiredIf(nameof(ElectricityDayDevicesFieldRequired), IfValue = true, ErrorMessage = "Please enter a valid meter reading")]
		[RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading")]
		public string MeterReadingDay { get; set; }
		[RequiredIf(nameof(ElectricityDayDevicesFieldRequired), IfValue = true, ErrorMessage = "Please enter a valid meter reading")]
		[RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading")]
		public string MeterReadingNight { get; set; }
		[RequiredIf(nameof(ElectricityNightStorageHeaterDevicesFieldRequired), IfValue = true, ErrorMessage = "Please enter a valid meter reading")]
		[RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading")]
		public string MeterReadingNightStorageHeater { get; set; }
		[RequiredIf(nameof(GasDevicesFieldRequired), IfValue = true, ErrorMessage = "Please enter a valid meter reading")]
		[RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading ")]
		public string MeterReadingGas { get; set; }
	}
}
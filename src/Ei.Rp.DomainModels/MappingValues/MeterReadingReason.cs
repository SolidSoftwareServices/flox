using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MeterReadingReason : TypedStringValue<MeterReadingReason>
	{
		[JsonConstructor]
		private MeterReadingReason()
		{
		}
		private MeterReadingReason(string value, string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue, true)
		{
		}
		public static readonly MeterReadingReason PeriodicMeterReading = new MeterReadingReason("01",  nameof(PeriodicMeterReading));
		public static readonly MeterReadingReason InterimMeterReadingWithBilling = new MeterReadingReason("02", nameof(InterimMeterReadingWithBilling));
		public static readonly MeterReadingReason MeterReadingAtMoveOut = new MeterReadingReason("03", nameof(MeterReadingAtMoveOut));
		public static readonly MeterReadingReason CertificationMeterReading = new MeterReadingReason("04", nameof(CertificationMeterReading));
		public static readonly MeterReadingReason ServiceTerritoryTransferWithBilling = new MeterReadingReason("05", nameof(ServiceTerritoryTransferWithBilling));
        public static readonly MeterReadingReason MeterReadingAtMoveIn = new MeterReadingReason("06", nameof(MeterReadingAtMoveIn));
        public static readonly MeterReadingReason ServiceTerritoryTransferWithoutBilling = new MeterReadingReason("07", nameof(ServiceTerritoryTransferWithoutBilling));
		public static readonly MeterReadingReason MeterReadingUponTechnicalInst = new MeterReadingReason("08", nameof(MeterReadingUponTechnicalInst));

        public static readonly MeterReadingReason InterimMeterReadingWithoutBilling = new MeterReadingReason("09", nameof(InterimMeterReadingWithoutBilling));
        public static readonly MeterReadingReason ControlReading = new MeterReadingReason("10", nameof(ControlReading));

        public static readonly MeterReadingReason MeterReadingAfterProgramAdjustment = new MeterReadingReason("11", nameof(MeterReadingAfterProgramAdjustment));
        public static readonly MeterReadingReason MeterReadingAtTechnicalRemoval = new MeterReadingReason("12", nameof(MeterReadingAtTechnicalRemoval));
        public static readonly MeterReadingReason MeterReadingOnDisconnection = new MeterReadingReason("13", nameof(MeterReadingOnDisconnection));
        public static readonly MeterReadingReason AutomaticallyCalculatedForProration = new MeterReadingReason("14", nameof(AutomaticallyCalculatedForProration));
        public static readonly MeterReadingReason StockTransferMeterReading = new MeterReadingReason("15", nameof(StockTransferMeterReading));
        public static readonly MeterReadingReason MeterReadingBeforeProgramAdjustment = new MeterReadingReason("16", nameof(MeterReadingBeforeProgramAdjustment));
        public static readonly MeterReadingReason ReadingForChangeOfInstStructure = new MeterReadingReason("17", nameof(ReadingForChangeOfInstStructure));
        public static readonly MeterReadingReason ReconnectionMeterReading = new MeterReadingReason("18", nameof(ReconnectionMeterReading));
        public static readonly MeterReadingReason DeliveryMeterReading = new MeterReadingReason("19", nameof(DeliveryMeterReading));
        public static readonly MeterReadingReason DeviceInspectionMeterReading = new MeterReadingReason("20", nameof(DeviceInspectionMeterReading));
        public static readonly MeterReadingReason MeterReadingAtBillingRelInst = new MeterReadingReason("21", nameof(MeterReadingAtBillingRelInst));
        public static readonly MeterReadingReason MeterReadingAtBillingRelRemoval = new MeterReadingReason("22", nameof(MeterReadingAtBillingRelRemoval));
        public static readonly MeterReadingReason SingleReadingForDeviceModification = new MeterReadingReason("23", nameof(SingleReadingForDeviceModification));
        public static readonly MeterReadingReason ReplacementUponInstallation = new MeterReadingReason("24", nameof(ReplacementUponInstallation));
        public static readonly MeterReadingReason ReplacementUponRemoval = new MeterReadingReason("25", nameof(ReplacementUponRemoval));
        public static readonly MeterReadingReason ContractChange = new MeterReadingReason("26", nameof(ContractChange));
        public static readonly MeterReadingReason CustomerChange = new MeterReadingReason("27", nameof(CustomerChange));
        public static readonly MeterReadingReason ReadingBeforeChangeToLogRegNo = new MeterReadingReason("28", nameof(ReadingBeforeChangeToLogRegNo));
        public static readonly MeterReadingReason ReadingAfterChangeToLogRegNo = new MeterReadingReason("29", nameof(ReadingAfterChangeToLogRegNo));
        public static readonly MeterReadingReason StartOfSimulation = new MeterReadingReason("90", nameof(StartOfSimulation));
        public static readonly MeterReadingReason EndOfSimulation = new MeterReadingReason("91", nameof(EndOfSimulation));
    }
}
using System;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Metering
{
    public class MeterReadingInfo : IQueryResult
    {
        public string DeviceId { get; set; }
        public string RegisterId { get; set; }

        public string SerialNumber { get; set; }

        public string MeterNumber { get; set; }
        public MeterType MeterType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? ReadingDate { get; set; }
        public decimal Reading { get; set; }
        public MeterReadingCategoryType MeterReadingCategory { get; set; }

        public MeterReadingStatus MeterReadingStatus { get; set; }
        public MeterReadingReason MeterReadingReason { get; set; }
        public decimal Consumption { get; set; }
        public string Unit { get; set; }
        public MeterUnit ReadingUnit { get; set; }
        public bool IsEstimate { get; set; }
        public bool IsPendingReadingVerification { get; set; }
        
        public decimal ConversionFactor { get; set; }
        public string MeasurementUnitForConsumption { get; set; }
        public ClientAccountType AccountType { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Vkont { get; set; } = string.Empty;

        public string Vertrag { get; set; } = string.Empty;

        public string Lcpe { get; set; } = string.Empty;

        public string FmoRequired { get; set; } = string.Empty;

        public string PchaRequired { get; set; } = string.Empty;
        public string MeterReadingReasonID { get; set; }
	    public DateTime ReadingDateTime { get; set; }

	
    }
}

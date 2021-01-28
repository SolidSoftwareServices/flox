using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Metering
{
	public class RegisterInfo : IQueryResult
	{
        public string DeviceId { get; set; }
        public UsePeriodType TimeofUsePeriod { get; set; }
        public ElectricityPointReferenceNumber MPRN { get; set; }
        public string RegisterId { get; set; }
    }
}
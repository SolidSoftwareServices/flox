using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Metering
{
	public class Premise : IQueryResult
	{
		public string PremiseId { get; set; }

		public AddressInfo Address { get; set; }
		public IEnumerable<PointOfDeliveryInfo> PointOfDeliveries { get; set; }
		public IEnumerable<InstallationInfo> Installations { get; set; }
		public string ElectricityDuosGroup { get; set; }
		public RegisterConfigType RegisterConfiguration { get; set; }
		public string ElectricityCTF { get; set; }
		public string ElectricityCTFOperandValue { get; set; }
		public string GasBand { get; set; }
	}
}
using System.Collections.Generic;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Metering
{
    public class InstallationInfo : IQueryResult
    {
		public virtual string InstallationId { get; set; }
        public virtual IEnumerable<DeviceInfo> Devices { get; set; } = new DeviceInfo[0];
        public virtual InstallationDiscStatusType DiscStatus { get; set; }
		public virtual DeregStatusType DeregStatus { get; set; }
		public virtual bool? HasFreeElectricityAllowance { get; set; }
		public string ElectricityDuosGroup { get; set; }
		public RegisterConfigType ElectricityRegisterConfiguration { get; set; }
		public CommsTechnicallyFeasibleValue ElectricityCtfValue { get; set; }
		public string GasBand { get; set; }
		public bool HasFirstStaffDiscount { get; set; }
    }
}
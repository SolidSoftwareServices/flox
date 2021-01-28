using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Platform;
using Ei.Rp.DomainModels.MappingValues;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.Metering
{
	public partial class DeviceInfo : IQueryResult
	{
		public string MeterReading { get; set; }

		public string ContractId { get; set; }

		public string DeviceId { get; set; }


		public string DivisionId { get; set; }

		public string DeviceMaterial { get; set; }

		public string DeviceLocation { get; set; }
		public string SerialNum { get; set; }
		public string DeviceDescription { get; set; }

		public CommsTechnicallyFeasibleValue CTF { get; set; }

		[Obsolete("use collection registers instead")]
		public MeterType MeterType { get; set; }
		[Obsolete("use collection registers instead")]
		public string MeterUnit { get; set; }


		public IEnumerable<MeterReadingInfo> MeterReadingResults { get; set; }
		public IEnumerable<DeviceRegisterInfo> Registers { get; set; }

		/// <summary>
		/// it defines whether a device is smart or not
		/// </summary>
		/// <remarks>This is independent of the smart activation status</remarks>
		public bool IsSmart { get; set; }

		public RegisterConfigType MCCConfiguration { get; set; }

		public string PremiseId { get; set; }

		public SmartActivationStatus SmartActivationStatus { get; set; }
	}
}

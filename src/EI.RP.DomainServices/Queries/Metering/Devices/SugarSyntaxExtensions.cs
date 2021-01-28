using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Metering.Devices
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<DeviceInfo>> GetDevicesByAccount(
			this IDomainQueryResolver provider,
			string accountNumber,
			bool byPassPipeline = false)
		{
            var query = new DevicesQuery
			{
				AccountNumber = accountNumber
			};

			return await provider
				.FetchAsync<DevicesQuery, DeviceInfo>(query, byPassPipeline);
		}

		public static async Task<IEnumerable<DeviceInfo>> GetDevicesByAccountAndContract(
			this IDomainQueryResolver provider,
			string accountNumber,
			string contractId,
			bool byPassPipeline = false)
		{
			var query = new DevicesQuery
			{
				AccountNumber = accountNumber,
				ContractId = contractId
			};

			return await provider
				.FetchAsync<DevicesQuery, DeviceInfo>(query, byPassPipeline);
		}

		public static async Task<DeviceInfo> GetDeviceById(this IDomainQueryResolver provider,
             string deviceId, bool byPassPipeline = false)
        {
            var query = new DevicesQuery
            {
                DeviceId = deviceId,
            };

			return (await provider
				.FetchAsync<DevicesQuery, DeviceInfo>(query, byPassPipeline)).Single();
		}
		public static async Task<DeviceInfo> GetNewPremiseDeviceById(this IDomainQueryResolver provider,
			PointReferenceNumber newPremisePrn, string deviceId, bool byPassPipeline = false)
		{
			var query = new DevicesQuery
			{
				PremisePrn = newPremisePrn,
				DeviceId = deviceId

			};

			return (await provider
				.FetchAsync<DevicesQuery, DeviceInfo>(query, byPassPipeline)).SingleOrDefault(x => x.DeviceId == deviceId);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Switch;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Register
{
	internal class RegisterInfoQueryHandler : QueryHandler<RegisterInfoQuery>
	{
		private readonly ISwitchDataRepository _switchDataRepository;

		public RegisterInfoQueryHandler(ISwitchDataRepository switchDataRepository)
		{
			_switchDataRepository = switchDataRepository;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(RegisterInfo)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			RegisterInfoQuery query)
		{
			try
			{
				var registerDetail = await _switchDataRepository.GetRegisterDetailFromMprn(query.Prn.ToString());
				return MapRegisterDetail(registerDetail.ToArray())
					.ToArray()
					.Cast<TQueryResult>();
			}
			catch (Exception)
			{
				return new List<TQueryResult>();
			}
		}

		private static IEnumerable<RegisterInfo> MapRegisterDetail(IEnumerable<RegisterDto> registerList)
		{
			return registerList.Select(registerDto => new RegisterInfo
			{
				MPRN = registerDto.MPRN,
				DeviceId = registerDto.Device,
				TimeofUsePeriod = (UsePeriodType) registerDto.TimeofUsePeriod
			}).ToList();
		}
	}
}
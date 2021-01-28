using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Infrastructure.Mappers;
using CrmPremiseDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.PremiseDto;
using ErpPremiseDto = EI.RP.DataModels.Sap.ErpUmc.Dtos.PremiseDto;
namespace EI.RP.DomainServices.Queries.Metering.Premises
{
	internal class PremisesQueryHandler : QueryHandler<PremisesQuery>
	{
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		private readonly IDomainMapper<ErpPremiseDto, CrmPremiseDto, Premise> _mapper;

		public PremisesQueryHandler(ISapRepositoryOfErpUmc erpUmc, ISapRepositoryOfCrmUmc crmUmc,IDomainMapper<ErpPremiseDto,CrmPremiseDto,Premise>mapper)
		{
			_erpUmc = erpUmc;
			_crmUmc = crmUmc;
			_mapper = mapper;
		}

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			PremisesQuery query)
		{
			Task<ErpPremiseDto> premiseDtoErpUmc;
			Task<CrmPremiseDto> premiseDtoCrmUmc;
			if (!string.IsNullOrWhiteSpace(query.PremiseId))
			{
				premiseDtoErpUmc = GetFromErpUmc(query.PremiseId);
				premiseDtoCrmUmc = GetFromCrmUmc(query.PremiseId);

			}
			else if (query.Prn != null)
			{
				premiseDtoCrmUmc = GetFromCrmUmc(query.Prn);
				premiseDtoErpUmc = GetFromErpUmc((await premiseDtoCrmUmc).PremiseID);
			}
			else
			{
				throw new ArgumentException();
			}

			var result = await _mapper.Map(await premiseDtoErpUmc, await premiseDtoCrmUmc);
			return result.ToOneItemArray().Cast<TQueryResult>();
		}

		private async Task<CrmPremiseDto> GetFromCrmUmc(PointReferenceNumber prn)
		{
			var queryTask = _crmUmc.NewQuery<PointOfDeliveryDto>()
				.Key(prn.ToString())
				.NavigateTo<CrmPremiseDto>()
				.Expand(x => x.PointOfDeliveries)
				.Expand(x => x.PointOfDeliveries[0].Premise)
				.GetMany();
			return (await queryTask).FirstOrDefault();
		}

		private async Task<CrmPremiseDto> GetFromCrmUmc(string premiseId)
		{
			var queryTask = _crmUmc.NewQuery<CrmPremiseDto>()
				.Key(premiseId)
				.Expand(x => x.PointOfDeliveries)
				.Expand(x => x.PointOfDeliveries[0].Premise)
				.GetMany();
			var premiseDto = (await queryTask).FirstOrDefault();
			return premiseDto;
		}
		private async Task<ErpPremiseDto> GetFromErpUmc(string premiseId)
		{
			var premiseDtos = await _erpUmc.NewQuery<ErpPremiseDto>()
				.Key(premiseId)
				.Expand(x => x.Installations)
				.Expand(x => x.Installations[0].Devices)
				.Expand(x => x.Installations[0].Devices[0].MeterReadingResults)
				.Expand(x => x.Installations[0].Devices[0].RegistersToRead)
				.Expand(x => x.Installations[0].Devices[0].RegistersToRead[0].RegisterType)
				.Expand(x => x.Installations[0].InstallationFacts)
				.GetMany();
			var premiseDto = premiseDtos.FirstOrDefault();
			return premiseDto;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(Premise) };
	}
}
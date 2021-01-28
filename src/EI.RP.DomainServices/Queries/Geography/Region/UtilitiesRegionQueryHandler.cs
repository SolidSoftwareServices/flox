using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.ReadOnlyCollections;

namespace EI.RP.DomainServices.Queries.Geography.Region
{
    internal class UtilitiesRegionQueryHandler : QueryHandler<UtilitiesRegionQuery>
    {
        private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;


        public UtilitiesRegionQueryHandler(ISapRepositoryOfCrmUmc crmUmcRepository)
        {
            _crmUmcRepository = crmUmcRepository;
        }

        protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(UtilitiesRegionQuery query)
        {

            var regionTask = _crmUmcRepository.NewQuery<RegionDto>()
                .Filter(x=>x.CountryID== query.CountryId,false)
                .GetMany();
            var result= await regionTask;
            return result.Select(Map).Cast<TQueryResult>();

        }

        private RegionDetails Map(RegionDto dto)
        {
            var region = new RegionDetails();
            region.CountryID = dto.CountryID;
            region.RegionID = dto.RegionID;
            region.Name = dto.Name;
            return region;
        }

        protected override Type[] ValidQueryResultTypes { get; } = { typeof(RegionDetails) };
    }
}

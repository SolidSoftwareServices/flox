using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.ReadOnlyCollections;

namespace EI.RP.DomainServices.Queries.Geography.Country
{
    internal class CountryQueryHandler : QueryHandler<CountryDetailsQuery>
    {
        private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;


        public CountryQueryHandler(ISapRepositoryOfCrmUmc crmUmcRepository)
        {
            _crmUmcRepository = crmUmcRepository;
        }

        protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(CountryDetailsQuery query)
        {
            IEnumerable<CountryDto> result;
            if (query.HasCountry())
            {
                result = (await _crmUmcRepository.NewQuery<CountryDto>()
                    .Key(query.CountryId)
                    .GetOne())?.ToOneItemArray()??new CountryDto[0];
            }
            else
            {
                result=await _crmUmcRepository.NewQuery<CountryDto>()
                    .GetMany();
            }

            return result.Select(Map).Cast<TQueryResult>();

        }

        private CountryDetails Map(CountryDto dto)
        {
            var country = new CountryDetails();
            country.CountryID = dto.CountryID;
            country.Name = dto.Name;
            return country;
        }

        protected override Type[] ValidQueryResultTypes { get; } = { typeof(CountryDetails) };
    }
}

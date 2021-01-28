using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Register
{
    public static class SugarSyntaxExtensions
    {
        public static async Task<RegisterInfo[]> GetRegisterInfoByPrn(
            this IDomainQueryResolver domainQueryResolver,
            PointReferenceNumber prn, bool byPassPipeline = false)
        {
            var result = await domainQueryResolver.FetchAsync<RegisterInfoQuery, RegisterInfo>(new RegisterInfoQuery
            {
                Prn = prn
            },byPassPipeline);

            return result.ToArray();
        }
    }
}
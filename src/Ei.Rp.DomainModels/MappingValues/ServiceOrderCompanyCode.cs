using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class ServiceOrderCompanyCode : TypedStringValue<ServiceOrderCompanyCode>
    {
    

        [JsonConstructor]
        private ServiceOrderCompanyCode()
        {
        }

        private ServiceOrderCompanyCode(string value) : base(value)
        {
        }

        public static readonly ServiceOrderCompanyCode SPLY = new ServiceOrderCompanyCode("SPLY");
    }
}

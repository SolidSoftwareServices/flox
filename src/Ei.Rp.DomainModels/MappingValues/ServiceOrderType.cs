using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class ServiceOrderType: TypedStringValue<ServiceOrderType>
    {
      

        [JsonConstructor]
        private ServiceOrderType()
        {
        }

        private ServiceOrderType(string value) : base(value)
        {
        }

        public static readonly ServiceOrderType Zm71 = new ServiceOrderType("ZM71");
    }
}

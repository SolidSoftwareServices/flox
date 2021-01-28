using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class CountryIdType : TypedStringValue<CountryIdType>
    {

        [JsonConstructor]
        private CountryIdType()
        {
        }

        private CountryIdType(string value) : base(value)
        {
        }

        public static readonly CountryIdType IE = new CountryIdType("IE");
        public static readonly CountryIdType GB = new CountryIdType("GB");
    }
}

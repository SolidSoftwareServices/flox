using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class Currency : TypedStringValue<Currency>
    {
        [JsonConstructor]
        private Currency()
        {
        }
        private Currency(string value) : base(value)
        {
        }

        public static readonly Currency Euro = new Currency("EUR");
    }
}

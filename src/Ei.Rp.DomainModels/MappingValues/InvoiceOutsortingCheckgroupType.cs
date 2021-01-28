using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class InvoiceOutsortingCheckgroupType : TypedStringValue<InvoiceOutsortingCheckgroupType>
    {

        [JsonConstructor]
        private InvoiceOutsortingCheckgroupType()
        {
        }

        private InvoiceOutsortingCheckgroupType(string value) : base(value)
        {
        }

        public static readonly InvoiceOutsortingCheckgroupType EQUAL = new InvoiceOutsortingCheckgroupType(nameof(EQUAL));
    }
}

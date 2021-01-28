using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class ContactQueryType : TypedStringValue<ContactQueryType>
    {
		[JsonConstructor]
        private ContactQueryType()
        {
        }

        public ContactQueryType(string value) : base(value)
        {
        }

        public static readonly ContactQueryType AddAdditionalAccount = new ContactQueryType("Add an additional account");
        public static readonly ContactQueryType BillOrPayment = new ContactQueryType("Bill or payment query");
        public static readonly ContactQueryType MeterRead = new ContactQueryType("Meter read query");
        public static readonly ContactQueryType Other = new ContactQueryType("Other");       
	}
}
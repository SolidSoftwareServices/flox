using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class PhoneType : TypedStringValue<PhoneType>
    {

        [JsonConstructor]
        private PhoneType()
        {
        }

        private PhoneType(string value) : base(value)
        {
        }

        public static readonly PhoneType DefaultMobilePhone = new PhoneType("3");
        public static readonly PhoneType NotDefaultMobilePhone = new PhoneType("2");
        public static readonly PhoneType DefaultLandlinePhone = new PhoneType("1");
    }
}

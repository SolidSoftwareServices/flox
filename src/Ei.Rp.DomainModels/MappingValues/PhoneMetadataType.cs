using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class PhoneMetadataType : TypedStringValue<PhoneMetadataType>
    {
        [JsonConstructor]
        private PhoneMetadataType()
        {
        }


        public PhoneMetadataType(string value, string debuggerFriendlyDisplayValue = null, bool disableVerifyValueExists = false) : base(value, debuggerFriendlyDisplayValue, disableVerifyValueExists)
        {
        }

        public static readonly PhoneMetadataType Mobile = new PhoneMetadataType("M", "mobile");
        public static readonly PhoneMetadataType LandLine = new PhoneMetadataType("L", "landline");
        public static readonly PhoneMetadataType Invalid = new PhoneMetadataType("Invalid", "Invalid");
    }
}

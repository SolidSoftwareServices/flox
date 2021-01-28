using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class CommunicationPreferenceType: TypedStringValue<CommunicationPreferenceType>
    {
        [JsonConstructor]
        private CommunicationPreferenceType()
        {
        }

        public CommunicationPreferenceType(string value, string debuggerFriendlyDisplayValue = null, bool disableVerifyValueExists = false) : base(value, debuggerFriendlyDisplayValue, disableVerifyValueExists)
        {
        }

        public static readonly CommunicationPreferenceType Email = new CommunicationPreferenceType("INT", "E-Mail");
        public static readonly CommunicationPreferenceType LandLine = new CommunicationPreferenceType("Z1L","Landline");
        public static readonly CommunicationPreferenceType Mobile = new CommunicationPreferenceType("Z2M","Mobile");
        public static readonly CommunicationPreferenceType Post = new CommunicationPreferenceType("Z3M","Post");
        public static readonly CommunicationPreferenceType SMS = new CommunicationPreferenceType("PAG","SMS");
        public static readonly CommunicationPreferenceType DoorToDoor = new CommunicationPreferenceType("Z5F","Door to Door");
    }
}

using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class CommunicationPermissionStatusType : TypedStringValue<CommunicationPermissionStatusType>
    {
        [JsonConstructor]
        private CommunicationPermissionStatusType()
        {
        }

        private CommunicationPermissionStatusType(string value) : base(value)
        {
        }

        public static readonly CommunicationPermissionStatusType Accepted = new CommunicationPermissionStatusType("001");
        public static readonly CommunicationPermissionStatusType NotAccepted = new CommunicationPermissionStatusType("002");
    }
}

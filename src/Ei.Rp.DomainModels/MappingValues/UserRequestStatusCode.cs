using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class UserRequestStatusCode : TypedStringValue<UserRequestStatusCode>
    {
        [JsonConstructor]
        private UserRequestStatusCode()
        {
        }

        private UserRequestStatusCode(string value) : base(value)
        {
        }

        public static readonly UserRequestStatusCode Open = new UserRequestStatusCode("00");
        public static readonly UserRequestStatusCode InProcess = new UserRequestStatusCode("01");
        public static readonly UserRequestStatusCode Cancelled = new UserRequestStatusCode("02");
        public static readonly UserRequestStatusCode Completed = new UserRequestStatusCode("03");
        public static readonly UserRequestStatusCode ToBeDeleted = new UserRequestStatusCode("04");
    }
}

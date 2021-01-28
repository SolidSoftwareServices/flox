using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class UserCategoryType : TypedStringValue<UserCategoryType>
    {

        [JsonConstructor]
        private UserCategoryType()
        {
        }

        private UserCategoryType(string value) : base(value)
        {
        }
        public static readonly UserCategoryType None = new UserCategoryType(string.Empty);
		public static readonly UserCategoryType AddAdditionalAccountUserCategory = new UserCategoryType("001");
    }
}

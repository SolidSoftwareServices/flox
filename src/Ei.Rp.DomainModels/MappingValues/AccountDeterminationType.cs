using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class AccountDeterminationType : TypedStringValue<AccountDeterminationType>
    {
        [JsonConstructor]
        private AccountDeterminationType()
        {
        }

        private AccountDeterminationType(string value) : base(value)
        {
        }

        public static readonly AccountDeterminationType ResidentialCustomer = new AccountDeterminationType("01");
        public static readonly AccountDeterminationType NonResidentialCustomer = new AccountDeterminationType("02");
        public static readonly AccountDeterminationType ServiceProvider = new AccountDeterminationType("03");
    }
}

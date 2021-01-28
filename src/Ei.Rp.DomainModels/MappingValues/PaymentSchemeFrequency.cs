using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class PaymentSchemeFrequency: TypedStringValue<PaymentSchemeFrequency>
    {
        [JsonConstructor]
        private PaymentSchemeFrequency()
        {
        }
        private PaymentSchemeFrequency(string value, string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue)
        {
        }

        public static readonly PaymentSchemeFrequency Monthly = new PaymentSchemeFrequency("M", nameof(Monthly));
    }
}

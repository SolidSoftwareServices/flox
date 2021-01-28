using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class PaymentSchemeCategoryType :TypedStringValue<PaymentSchemeCategoryType>
    {
    [JsonConstructor]
    private PaymentSchemeCategoryType()
    {
    }
    private PaymentSchemeCategoryType(string value) : base(value)
    {
    }

    public static readonly PaymentSchemeCategoryType MEQCategoryType = new PaymentSchemeCategoryType("MEQ");
    }
}

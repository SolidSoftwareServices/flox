using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class BudgetBillingProcedureType : TypedStringValue<BudgetBillingProcedureType>
    {

        [JsonConstructor]
        private BudgetBillingProcedureType()
        {
        }

        private BudgetBillingProcedureType(string value) : base(value)
        {
        }

        public static readonly BudgetBillingProcedureType EqualizerPaymentMonthlyPayment = new BudgetBillingProcedureType("4");
    }
}

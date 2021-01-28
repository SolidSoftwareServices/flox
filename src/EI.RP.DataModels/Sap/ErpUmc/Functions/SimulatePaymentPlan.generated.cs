using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;

namespace EI.RP.DataModels.Sap.ErpUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class SimulatePaymentPlanFunction : ODataFunction<SimulatePaymentPlanFunction.QueryObject, PaymentPlanDto>
    {
        public SimulatePaymentPlanFunction(params Expression<Func<PaymentPlanDto, object>>[] expands): base("SimulatePaymentPlan", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(4)]
            public virtual string Year
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(2)]
            public virtual string StartingMonth
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string ContractID
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
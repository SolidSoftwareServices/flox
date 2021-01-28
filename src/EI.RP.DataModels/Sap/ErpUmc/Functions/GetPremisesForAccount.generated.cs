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
    public partial class GetPremisesForAccountFunction : ODataFunction<GetPremisesForAccountFunction.QueryObject, PremiseDto>
    {
        public GetPremisesForAccountFunction(params Expression<Func<PremiseDto, object>>[] expands): base("GetPremisesForAccount", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => true;
        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(10)]
            public virtual string AccountID
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
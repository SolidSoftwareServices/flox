using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class GetTransactionStatusesFunction : ODataFunction<GetTransactionStatusesFunction.QueryObject, DocumentStatusDto>
    {
        public GetTransactionStatusesFunction(params Expression<Func<DocumentStatusDto, object>>[] expands): base("GetTransactionStatuses", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => true;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            public virtual Guid TransactionGUID
            {
                get;
                set;
            }
        }
    }
}
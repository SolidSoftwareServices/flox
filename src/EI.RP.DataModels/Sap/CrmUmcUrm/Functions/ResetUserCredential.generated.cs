using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmcUrm.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class ResetUserCredentialFunction : ODataFunction<ResetUserCredentialFunction.QueryObject, UserCredentialResetStatusDto>
    {
        public ResetUserCredentialFunction(params Expression<Func<UserCredentialResetStatusDto, object>>[] expands): base("ResetUserCredential", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        public class QueryObject
        {
            [StringLength(60)]
            public virtual string UserName
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
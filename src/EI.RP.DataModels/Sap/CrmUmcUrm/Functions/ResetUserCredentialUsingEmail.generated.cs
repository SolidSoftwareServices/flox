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
    public partial class ResetUserCredentialUsingEmailFunction : ODataFunction<ResetUserCredentialUsingEmailFunction.QueryObject, UserCredentialResetUsingEmailDto>
    {
        public ResetUserCredentialUsingEmailFunction(params Expression<Func<UserCredentialResetUsingEmailDto, object>>[] expands): base("ResetUserCredentialUsingEmail", expands)
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
            [StringLength(241)]
            public virtual string UserEmailID
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
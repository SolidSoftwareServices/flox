using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.UserManagement.Dtos;

namespace EI.RP.DataModels.Sap.UserManagement.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class CurrentUserFunction : ODataFunction<CurrentUserFunction.QueryObject, CurrentUserDto>
    {
        public CurrentUserFunction(params Expression<Func<CurrentUserDto, object>>[] expands): base("CurrentUser", expands)
        {
        }

        public override bool ReturnsComplexType() => true;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        public class QueryObject
        {
        }
    }
}
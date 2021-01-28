using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.UserManagement.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class CurrentUserDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string UserName
        {
            get;
            set;
        }
    }
}
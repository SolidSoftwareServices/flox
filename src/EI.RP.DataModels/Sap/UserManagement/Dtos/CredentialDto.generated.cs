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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class CredentialDto : ODataDtoModel
    {
        public override string CollectionName() => "CredentialCollection";
        public override object[] UniqueId()
        {
            return new object[]{UserName, };
        }

        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string UserName
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string Password
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string CurrentPassword
        {
            get;
            set;
        }
    }
}
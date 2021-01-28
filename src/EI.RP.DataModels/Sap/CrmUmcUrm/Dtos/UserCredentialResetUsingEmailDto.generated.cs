using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmcUrm.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class UserCredentialResetUsingEmailDto : ODataDtoModel
    {
        public override string CollectionName() => "UserCredentialResetUsingEmailCollection";
        public override object[] UniqueId()
        {
            return new object[]{UserEmailID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(241)]
        [Sortable]
        [Filterable]
        public virtual string UserEmailID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual bool? ResetCredentials
        {
            get;
            set;
        }
    }
}
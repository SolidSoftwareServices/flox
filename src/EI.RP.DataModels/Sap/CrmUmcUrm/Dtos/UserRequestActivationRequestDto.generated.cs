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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class UserRequestActivationRequestDto : ODataDtoModel
    {
        public override string CollectionName() => "UserRequestActivationRequestCollection";
        public override object[] UniqueId()
        {
            return new object[]{RequestID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        [Sortable]
        [Filterable]
        public virtual string RequestID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string ActivationKey
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string Password
        {
            get;
            set;
        }
    }
}
using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class CommunicationPermissionStatusDto : ODataDtoModel
    {
        public override string CollectionName() => "CommunicationPermissionStatuses";
        public override object[] UniqueId()
        {
            return new object[]{CommunicationPermissionStatusID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string CommunicationPermissionStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(60)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
    }
}
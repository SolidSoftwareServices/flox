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
    public partial class DocumentStatusDto : ODataDtoModel
    {
        public override string CollectionName() => "DocumentStatuses";
        public override object[] UniqueId()
        {
            return new object[]{DocumentStatusID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(13)]
        public virtual string DocumentStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(30)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string InternalID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(8)]
        public virtual string ProfileID
        {
            get;
            set;
        }

        [StringLength(2)]
        public virtual string Priority
        {
            get;
            set;
        }
    }
}
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
    [CrudOperations(CanAdd = true, CanUpdate = true, CanDelete = true, CanQuery = true)]
    public partial class RequestDetailsConfirmationDto : ODataDtoModel
    {
        public override string CollectionName() => "RequestDetailsConfirmationCollection";
        public override object[] UniqueId()
        {
            return new object[]{ID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        [Sortable]
        [Filterable]
        public virtual string ID
        {
            get;
            set;
        }

        = string.Empty;
    }
}
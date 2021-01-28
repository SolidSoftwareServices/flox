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
    public partial class UserCategoryDto : ODataDtoModel
    {
        public override string CollectionName() => "UserCategoryCollection";
        public override object[] UniqueId()
        {
            return new object[]{ID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Sortable]
        [Filterable]
        public virtual string ID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(255)]
        [Sortable]
        [Filterable]
        public virtual string Description
        {
            get;
            set;
        }
    }
}
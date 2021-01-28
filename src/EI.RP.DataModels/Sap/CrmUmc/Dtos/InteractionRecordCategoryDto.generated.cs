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
    public partial class InteractionRecordCategoryDto : ODataDtoModel
    {
        public override string CollectionName() => "InteractionRecordCategories";
        public override object[] UniqueId()
        {
            return new object[]{InteractionRecordCategoryGUID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(22)]
        public virtual string InteractionRecordCategoryGUID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(22)]
        public virtual string InteractionRecordCategoryParentGUID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<InteractionRecordCategoryDto> InteractionRecordSubCategories
        {
            get;
            set;
        }

        = new List<InteractionRecordCategoryDto>();
    }
}
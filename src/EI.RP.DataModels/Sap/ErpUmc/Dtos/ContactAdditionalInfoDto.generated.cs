using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ContactAdditionalInfoDto : ODataDtoModel
    {
        public override string CollectionName() => "ContactAdditionalInfos";
        public override object[] UniqueId()
        {
            return new object[]{ContactAdditionalInfoID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ContactAdditionalInfoID
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
    }
}
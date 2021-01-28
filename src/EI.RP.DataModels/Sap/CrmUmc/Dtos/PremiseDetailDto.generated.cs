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
    [CrudOperations(CanAdd = true, CanUpdate = true, CanDelete = true, CanQuery = true)]
    public partial class PremiseDetailDto : ODataDtoModel
    {
        public override string CollectionName() => "PremiseDetails";
        public override object[] UniqueId()
        {
            return new object[]{PremiseID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(1000)]
        public virtual string PremiseNotes
        {
            get;
            set;
        }
    }
}
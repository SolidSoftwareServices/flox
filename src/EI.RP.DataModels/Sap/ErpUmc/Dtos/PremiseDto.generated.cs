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
    public partial class PremiseDto : ODataDtoModel
    {
        public override string CollectionName() => "Premises";
        public override object[] UniqueId()
        {
            return new object[]{PremiseID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Sortable]
        [Filterable]
        public virtual AddressInfoDto AddressInfo
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(8)]
        public virtual string PremiseTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual PremiseTypeDto PremiseType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<InstallationDto> Installations
        {
            get;
            set;
        }

        = new List<InstallationDto>();
    }
}
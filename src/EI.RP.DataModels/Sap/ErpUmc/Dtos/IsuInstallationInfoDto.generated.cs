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
    public partial class IsuInstallationInfoDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string InstallationID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string Sparte
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ContractID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DeregStat
        {
            get;
            set;
        }

        = string.Empty;
    }
}
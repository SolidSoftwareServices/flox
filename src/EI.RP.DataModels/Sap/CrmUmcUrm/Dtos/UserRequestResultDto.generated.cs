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
    public partial class UserRequestResultDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string BuAgID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? Birthday
        {
            get => _Birthday;
            set => _Birthday = CompensateSapDateTimeBug(value);
        }

        private DateTime? _Birthday;
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public virtual string PD_EXT_UI
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OWNER_FLAG
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string TC_FLAG
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string USERNAME
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string STATUS_CODE
        {
            get;
            set;
        }

        = string.Empty;
    }
}
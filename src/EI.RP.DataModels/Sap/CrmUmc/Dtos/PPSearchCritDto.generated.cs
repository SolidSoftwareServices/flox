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
    public partial class PPSearchCritDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string ONLINE
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string DUELFUEL
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string CREDITWORTH
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string CUSTOMERSCORE
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string EX_GRATIA
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string XDIVISION01
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string IRECTDEBIT_ELEC
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string DUOS_GROUP
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string METERTYPE_ELEC
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string SALESTYPE_E
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string QUARTERLYHOUR
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string E_REG_CONFIG
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string XDIVISION02
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string DIRECTDEBIT_GAS
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string BAND
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string METERTYPE_GAS
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string SALESTYPE_G
        {
            get;
            set;
        }

        = string.Empty;
    }
}
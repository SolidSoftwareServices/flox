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
    public partial class ProductProposalParamsDto : ODataDtoModel
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
        [StringLength(10)]
        public virtual string SalesOrigin
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool PaperlessBilling
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string BonusAmount
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool ElecFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ElecBusinessAgreementID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string ElecSalesType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string ElecDuOSGroup
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool ElecDirectDebit
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ElecMeterType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ElecRegisterConfig
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ElecCTF
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool ElecQuarterHourly
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual string ElecAnnualConsumption
        {
            get;
            set;
        }

        = "0.0";
        [Required(AllowEmptyStrings = true)]
        public virtual bool GasFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string GasBusinessAgreementID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string GasSalesType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string GasBand
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool GasDirectDebit
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string GasMeterType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual string GasAnnualConsumption
        {
            get;
            set;
        }

        = "0.0";
    }
}
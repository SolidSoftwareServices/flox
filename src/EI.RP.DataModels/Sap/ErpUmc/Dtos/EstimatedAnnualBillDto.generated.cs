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
    public partial class EstimatedAnnualBillDto : ODataDtoModel
    {
        public override string CollectionName() => "EstimatedAnnualBills";
        public override object[] UniqueId()
        {
            return new object[]{ProductID, ValidFor, MCC, DivisionGroup, DiscountID, FEA, PAYGInstallationFee, PAYGYear1ServiceCharge, PAYGOtherServiceCharge, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        [Filterable]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(7)]
        [Filterable]
        public virtual string ValidFor
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string MCC
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        [Filterable]
        public virtual string DivisionGroup
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        [Filterable]
        public virtual string DiscountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual bool FEA
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual bool PAYGInstallationFee
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PAYGYear1ServiceCharge
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PAYGOtherServiceCharge
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string RateCategory
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string MonthNoticeLetterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AnnualLetterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Units
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal StandingCharge
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal NSHStandingCharge
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal EnergyCharges
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal OtherCharges
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal PrepayRate
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal MeterRate1
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal MeterRate2
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal MeterRate3
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal MeterRate4
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal TotalAmount
        {
            get;
            set;
        }
    }
}
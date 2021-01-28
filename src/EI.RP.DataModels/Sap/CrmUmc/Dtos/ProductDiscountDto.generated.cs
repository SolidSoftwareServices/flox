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
    public partial class ProductDiscountDto : ODataDtoModel
    {
        public override string CollectionName() => "ProductDiscounts";
        public override object[] UniqueId()
        {
            return new object[]{ProductID, DiscountID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
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
        [StringLength(4)]
        [Filterable]
        public virtual string DiscountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal DiscountAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(30)]
        public virtual string Name
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(250)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool DirectDebitFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual bool PaperlessBillingFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual bool PayOnTimeFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual bool DualFuelFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(60)]
        public virtual string DiscountBasis
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(60)]
        public virtual string DiscountMethod
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual bool Active
        {
            get;
            set;
        }

        [Filterable]
        public virtual DateTime? FromDate
        {
            get => _FromDate;
            set => _FromDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _FromDate;
        [Filterable]
        public virtual DateTime? ToDate
        {
            get => _ToDate;
            set => _ToDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ToDate;
    }
}
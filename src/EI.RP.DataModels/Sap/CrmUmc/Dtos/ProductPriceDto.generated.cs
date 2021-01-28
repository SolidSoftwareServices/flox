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
    public partial class ProductPriceDto : ODataDtoModel
    {
        public override string CollectionName() => "ProductPrices";
        public override object[] UniqueId()
        {
            return new object[]{ProductID, DivisionID, ProductGroupID, RateCategory, OperandID, PriceValidTo, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(54)]
        [Filterable]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        [Filterable]
        public virtual string DivisionID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        [Filterable]
        public virtual string ProductGroupID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string RateCategory
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string OperandID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual DateTime PriceValidTo
        {
            get => _PriceValidTo;
            set => _PriceValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime _PriceValidTo;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PriceID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string PriceCategory
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime PriceValidFrom
        {
            get => _PriceValidFrom;
            set => _PriceValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime _PriceValidFrom;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal PriceAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(30)]
        public virtual string OperandDescription
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public virtual string PriceDescription
        {
            get;
            set;
        }

        = string.Empty;
    }
}
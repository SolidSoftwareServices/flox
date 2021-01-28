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
    public partial class ProductDto : ODataDtoModel
    {
        public override string CollectionName() => "Products";
        public override object[] UniqueId()
        {
            return new object[]{ProductID, };
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
        [StringLength(40)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1320)]
        public virtual string LongText
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4096)]
        public virtual string IconURL
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
        [StringLength(8)]
        [Filterable]
        public virtual string PremiseTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        [Filterable]
        public virtual string PersonsMin
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        [Filterable]
        public virtual string PersonsMax
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        [Filterable]
        public virtual string ConsumptionMin
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        [Filterable]
        public virtual string ConsumptionMax
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ProductGroupID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<UtilitiesProductPriceDto> UtilitiesProductPrices
        {
            get;
            set;
        }

        = new List<UtilitiesProductPriceDto>();
        [Sortable]
        [Filterable]
        public virtual PremiseTypeDto PremiseType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DivisionDto Division
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<CRMIsuDiscountDto> CRMIsuDiscounts
        {
            get;
            set;
        }

        = new List<CRMIsuDiscountDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProductDiscountDto> Discounts
        {
            get;
            set;
        }

        = new List<ProductDiscountDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProductAttributeDto> Attributes
        {
            get;
            set;
        }

        = new List<ProductAttributeDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProductPriceDto> Prices
        {
            get;
            set;
        }

        = new List<ProductPriceDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProductBundleDto> Bundles
        {
            get;
            set;
        }

        = new List<ProductBundleDto>();
    }
}
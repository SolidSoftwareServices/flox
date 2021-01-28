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
    public partial class UtilitiesProductPriceDto : ODataDtoModel
    {
        public override string CollectionName() => "UtilitiesProductPrices";
        public override object[] UniqueId()
        {
            return new object[]{ProductID, ConditionType, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(54)]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string ConditionType
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Price
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string Currency
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Quantity
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string QuantityUnit
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual PriceConditionTypeDto PriceConditionType
        {
            get;
            set;
        }
    }
}
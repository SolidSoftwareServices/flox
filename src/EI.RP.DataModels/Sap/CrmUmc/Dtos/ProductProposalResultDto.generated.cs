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
    public partial class ProductProposalResultDto : ODataDtoModel
    {
        public override string CollectionName() => "ProductProposalResults";
        public override object[] UniqueId()
        {
            return new object[]{BundleID, ElecProductID, GasProductID, ElecDiscountID, GasDiscountID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Sortable]
        [Filterable]
        public virtual ProductProposalParamsDto SearchParameters
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string BundleID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string ElecProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string GasProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string ElecDiscountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string GasDiscountID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual ProductDto ElecProduct
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProductDto GasProduct
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProductDiscountDto ElecDiscount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProductDiscountDto GasDiscount
        {
            get;
            set;
        }
    }
}
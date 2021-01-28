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
    public partial class ContractItemDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractItems";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ContractID
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
        public virtual Guid? ContractItemGUID
        {
            get;
            set;
        }

        public virtual Guid? ContractHeaderGUID
        {
            get;
            set;
        }

        [Filterable]
        public virtual DateTime? ContractStartDate
        {
            get => _ContractStartDate;
            set => _ContractStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ContractStartDate;
        public virtual DateTime? ContractEndDate
        {
            get => _ContractEndDate;
            set => _ContractEndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ContractEndDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string BusinessAgreementID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DivisionID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(22)]
        public virtual string PointOfDeliveryGUID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(10)]
        public virtual string FormerServiceProviderID
        {
            get;
            set;
        }

        [StringLength(10)]
        public virtual string NewServiceProviderID
        {
            get;
            set;
        }

        [StringLength(10)]
        public virtual string Distributor
        {
            get;
            set;
        }

        [StringLength(1)]
        public virtual string SwitchServiceFlag
        {
            get;
            set;
        }

        [StringLength(35)]
        public virtual string MeterNumber
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PremiseDto Premise
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProductDto Product
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractItemTimeSliceDto> ContractItemTimeSlices
        {
            get;
            set;
        }

        = new List<ContractItemTimeSliceDto>();
        [Sortable]
        [Filterable]
        public virtual BusinessAgreementDto BusinessAgreement
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
        public virtual ContractItemEXTAttrDto ContractItemEXTAttrs
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<SmartConsentDto> SmartConsents
        {
            get;
            set;
        }

        = new List<SmartConsentDto>();
        [Sortable]
        [Filterable]
        public virtual List<ContractAttributeDto> Attributes
        {
            get;
            set;
        }

        = new List<ContractAttributeDto>();
        [Sortable]
        [Filterable]
        public virtual List<ContractDateDto> Dates
        {
            get;
            set;
        }

        = new List<ContractDateDto>();
    }
}
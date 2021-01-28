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
    public partial class ContractItemTimeSliceDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractItemTimeSlices";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, StartDate, };
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
        [StringLength(40)]
        public virtual string Description
        {
            get;
            set;
        }

        public virtual Guid? ContractItemTimeSliceGUID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual DateTime StartDate
        {
            get => _StartDate;
            set => _StartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _StartDate;
        public virtual DateTime? EndDate
        {
            get => _EndDate;
            set => _EndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _EndDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string ProductID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(13)]
        public virtual string DocumentStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual ProductDto Product
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ContractItemDto ContractItem
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DocumentStatusDto DocumentStatus
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
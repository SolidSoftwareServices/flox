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
    public partial class ContractDto : ODataDtoModel
    {
        public override string CollectionName() => "Contracts";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
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
        [StringLength(35)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ContractAccountID
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
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? MoveInDate
        {
            get => _MoveInDate;
            set => _MoveInDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _MoveInDate;
        public virtual DateTime? MoveOutDate
        {
            get => _MoveOutDate;
            set => _MoveOutDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _MoveOutDate;
        [StringLength(50)]
        public virtual string ProcessingStatus
        {
            get;
            set;
        }

        [Filterable]
        public virtual bool? HistoricFlag
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentPlanDto ActivePaymentPlan
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractConsumptionValueDto> ContractConsumptionValues
        {
            get;
            set;
        }

        = new List<ContractConsumptionValueDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProfileValuesFileDto> ProfileValuesFiles
        {
            get;
            set;
        }

        = new List<ProfileValuesFileDto>();
        [Sortable]
        [Filterable]
        public virtual PremiseDto Premise
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<MeterReadingResultDto> MeterReadingResults
        {
            get;
            set;
        }

        = new List<MeterReadingResultDto>();
        [Sortable]
        [Filterable]
        public virtual List<DeviceDto> Devices
        {
            get;
            set;
        }

        = new List<DeviceDto>();
        [Sortable]
        [Filterable]
        public virtual BudgetBillingPlanDto ActiveBudgetBillingPlan
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractAllDeviceDto> ContractAllDevices
        {
            get;
            set;
        }

        = new List<ContractAllDeviceDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProfileValueDto> ProfileValues
        {
            get;
            set;
        }

        = new List<ProfileValueDto>();
        [Sortable]
        [Filterable]
        public virtual DivisionDto Division
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ContractAccountDto ContractAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentSchemeDto ActivePaymentScheme
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ProfileValueCostDto> ProfileValueCosts
        {
            get;
            set;
        }

        = new List<ProfileValueCostDto>();
    }
}
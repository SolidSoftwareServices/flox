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
    public partial class ContractConsumptionValueDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractConsumptionValues";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, ConsumptionPeriodTypeID, StartDate, };
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
        [StringLength(2)]
        public virtual string ConsumptionPeriodTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime StartDate
        {
            get => _StartDate;
            set => _StartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _StartDate;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime EndDate
        {
            get => _EndDate;
            set => _EndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _EndDate;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal BilledAmount
        {
            get;
            set;
        }

        [StringLength(5)]
        public virtual string Currency
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal ConsumptionValue
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ConsumptionUnit
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string BillingPeriodYear
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string BillingPeriodMonth
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string MeterReadingCategoryID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual MeterReadingCategoryDto MeterReadingCategory
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ConsumptionPeriodTypeDto ConsumptionPeriodType
        {
            get;
            set;
        }
    }
}
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
    public partial class BudgetBillingPlanDto : ODataDtoModel
    {
        public override string CollectionName() => "BudgetBillingPlans";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, BudgetBillingPlanID, };
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
        [StringLength(12)]
        public virtual string BudgetBillingPlanID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string BudgetBillingCycleID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Amount
        {
            get;
            set;
        }

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
        [StringLength(5)]
        public virtual string Currency
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ChangeFromDate
        {
            get => _ChangeFromDate;
            set => _ChangeFromDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _ChangeFromDate;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal UpperLimitAmount
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal LowerLimitAmount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BudgetBillingCycleDto BudgetBillingCycle
        {
            get;
            set;
        }
    }
}
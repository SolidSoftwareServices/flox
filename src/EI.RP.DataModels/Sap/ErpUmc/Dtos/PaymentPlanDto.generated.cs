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
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = true, CanQuery = true)]
    public partial class PaymentPlanDto : ODataDtoModel
    {
        public override string CollectionName() => "PaymentPlans";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, PaymentPlanID, };
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
        public virtual string PaymentPlanID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string PaymentPlanCycleID
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
        public virtual decimal BalanceForwardAmount
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
        [StringLength(2)]
        public virtual string StartingMonth
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string Year
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string AlternativeStartingMonth
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual BudgetBillingCycleDto BudgetBillingCycle
        {
            get;
            set;
        }
    }
}
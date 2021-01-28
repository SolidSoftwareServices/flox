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
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class PaymentSchemeDto : ODataDtoModel
    {
        public override string CollectionName() => "PaymentSchemes";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, PaymentSchemeID, };
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
        public virtual string PaymentSchemeID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? StartDate
        {
            get => _StartDate;
            set => _StartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _StartDate;
        public virtual DateTime? FirstDueDate
        {
            get => _FirstDueDate;
            set => _FirstDueDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _FirstDueDate;
        public virtual DateTime? NextDueDate
        {
            get => _NextDueDate;
            set => _NextDueDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _NextDueDate;
        public virtual DateTime? AlternativePayDate
        {
            get => _AlternativePayDate;
            set => _AlternativePayDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _AlternativePayDate;
        public virtual decimal? Amount
        {
            get;
            set;
        }

        [StringLength(1)]
        public virtual string Frequency
        {
            get;
            set;
        }

        [StringLength(3)]
        public virtual string Category
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

        [StringLength(2)]
        public virtual string Status
        {
            get;
            set;
        }
    }
}
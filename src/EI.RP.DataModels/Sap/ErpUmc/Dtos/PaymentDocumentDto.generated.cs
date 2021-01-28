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
    public partial class PaymentDocumentDto : ODataDtoModel
    {
        public override string CollectionName() => "PaymentDocuments";
        public override object[] UniqueId()
        {
            return new object[]{PaymentDocumentID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(24)]
        public virtual string PaymentDocumentID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(140)]
        public virtual string PaymentMethodDescription
        {
            get;
            set;
        }

        = string.Empty;
        [Filterable]
        public virtual DateTime? ExecutionDate
        {
            get => _ExecutionDate;
            set => _ExecutionDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ExecutionDate;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Amount
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
        [Filterable]
        public virtual string PaymentStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string BankAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string PaymentCardID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string CVC
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string PaymentCardTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(25)]
        public virtual string CardNumber
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? ValidFrom
        {
            get => _ValidFrom;
            set => _ValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidFrom;
        public virtual DateTime? ValidTo
        {
            get => _ValidTo;
            set => _ValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidTo;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Cardholder
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
        [StringLength(1024)]
        public virtual string PaymentDocumentText
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual PaymentStatusDto PaymentDocumentStatus
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<PaymentDocumentItemDto> PaymentDocumentItems
        {
            get;
            set;
        }

        = new List<PaymentDocumentItemDto>();
    }
}
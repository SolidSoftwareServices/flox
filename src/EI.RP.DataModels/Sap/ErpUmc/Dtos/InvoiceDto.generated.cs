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
    public partial class InvoiceDto : ODataDtoModel
    {
        public override string CollectionName() => "Invoices";
        public override object[] UniqueId()
        {
            return new object[]{InvoiceID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string InvoiceID
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
        [StringLength(12)]
        public virtual string ContractAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal AmountDue
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
        public virtual DateTime DueDate
        {
            get => _DueDate;
            set => _DueDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _DueDate;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual DateTime InvoiceDate
        {
            get => _InvoiceDate;
            set => _InvoiceDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _InvoiceDate;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal AmountPaid
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual decimal AmountRemaining
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(140)]
        public virtual string InvoiceDescription
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        [Filterable]
        public virtual string InvoiceStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(12)]
        public virtual string SubstituteDocument
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(1024)]
        public virtual string InvoiceText
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(16)]
        public virtual string ReferenceDocNo
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? PrintDate
        {
            get => _PrintDate;
            set => _PrintDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _PrintDate;
        [StringLength(2)]
        public virtual string Division
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
        public virtual InvoicePDFDto InvoicePDF
        {
            get;
            set;
        }
    }
}
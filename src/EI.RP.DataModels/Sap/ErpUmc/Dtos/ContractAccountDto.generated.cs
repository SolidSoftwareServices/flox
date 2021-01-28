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
    [CrudOperations(CanAdd = true, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class ContractAccountDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractAccounts";
        public override object[] UniqueId()
        {
            return new object[]{ContractAccountID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ContractAccountID
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
        [StringLength(35)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string BudgetBillingProcedure
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string IncomingPaymentMethodID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OutgoingPaymentMethodID1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OutgoingPaymentMethodID2
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OutgoingPaymentMethodID3
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OutgoingPaymentMethodID4
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string OutgoingPaymentMethodID5
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string IncomingPaymentBankAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string OutgoingPaymentBankAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string IncomingPaymentPaymentCardID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string OutgoingPaymentPaymentCardID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountAddressID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string BillToAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string BillToAccountAddressID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AlternativePayeeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AlternativePayerID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string CountryID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<InvoiceDto> Invoices
        {
            get;
            set;
        }

        = new List<InvoiceDto>();
        [Sortable]
        [Filterable]
        public virtual ContractAccountBalanceDto ContractAccountBalance
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractDto> Contracts
        {
            get;
            set;
        }

        = new List<ContractDto>();
        [Sortable]
        [Filterable]
        public virtual AccountAddressDto BillToAccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto OutgoingPaymentMethod1
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentCardDto OutgoingAlternativePayeePaymentCard
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto IncomingPaymentMethod
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto OutgoingPaymentMethod3
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto OutgoingPaymentMethod5
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BankAccountDto IncomingBankAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountDto BillToAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BankAccountDto OutgoingAlternativePayeeBankAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountDto AlternativePayee
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BankAccountDto IncomingAlternativePayerBankAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountAddressDto AccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual CountryDto Country
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentCardDto IncomingAlternativePayerPaymentCard
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<CommunicationPreferenceDto> CommunicationPreferences
        {
            get;
            set;
        }

        = new List<CommunicationPreferenceDto>();
        [Sortable]
        [Filterable]
        public virtual PaymentCardDto IncomingPaymentCard
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto OutgoingPaymentMethod4
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BankAccountDto OutgoingBankAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentCardDto OutgoingPaymentCard
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual PaymentMethodDto OutgoingPaymentMethod2
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountDto AlternativePayer
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<PaymentDocumentDto> PaymentDocuments
        {
            get;
            set;
        }

        = new List<PaymentDocumentDto>();
    }
}
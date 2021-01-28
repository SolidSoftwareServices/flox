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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class BusinessAgreementDto : ODataDtoModel
    {
        public override string CollectionName() => "BusinessAgreements";
        public override object[] UniqueId()
        {
            return new object[]{BusinessAgreementID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string BusinessAgreementID
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
        [StringLength(30)]
        public virtual string Description
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
        public virtual string BillToAccountAddressID
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
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AlternativePayerID
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
        [StringLength(2)]
        public virtual string FixedBillingDate
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string AccountCategory
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        [Filterable]
        public virtual string CollectiveParentID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string RewardsFlag
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string AccountClass
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string Language
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string PaperBill
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string EBiller
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(1)]
        public virtual string SEPAFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string AccountDeterminationID
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
        [StringLength(8)]
        public virtual string InvoiceOutsortingCheckGroup
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string AlternativePayerCA
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string FixedPaymentDate
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual AccountDto BillToAccount
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractItemDto> ContractItems
        {
            get;
            set;
        }

        = new List<ContractItemDto>();
        [Sortable]
        [Filterable]
        public virtual PaymentCardDto IncomingAlternativePayerPaymentCard
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
        public virtual PaymentMethodDto OutgoingPaymentMethod5
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
        public virtual PaymentCardDto OutgoingAlternativePayeePaymentCard
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
        public virtual BankAccountDto OutgoingBankAccount
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
        public virtual PaymentCardDto PaymentCard
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
        public virtual BankAccountDto IncomingBankAccount
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
        public virtual PaymentMethodDto OutgoingPaymentMethod4
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
        public virtual PaymentCardDto IncomingPaymentCard
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
        public virtual AccountDto AlternativePayee
        {
            get;
            set;
        }

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
        public virtual AccountAddressDto AccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountDto Account
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BusinessAgreementDto CollectiveParent
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual BusinessAgreementDto AlternativePayerBuAg
        {
            get;
            set;
        }
    }
}